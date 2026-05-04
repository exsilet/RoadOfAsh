using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RoadOfAsh.Scripts.Domain;
using RoadOfAsh.Scripts.Domain.Battle;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Map;
using RoadOfAsh.Scripts.Domain.Players;
using RoadOfAsh.Scripts.Domain.Rewards;
using RoadOfAsh.Scripts.Infrastructure;
using RoadOfAsh.Scripts.Presentation.Rewards;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class BattleScreen : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Transform handRoot;
        [SerializeField] private Button endTurnButton;
        [SerializeField] private TMP_Text playerHpText;
        [SerializeField] private TMP_Text enemyHpText;
        [SerializeField] private TMP_Text playerEnergyText;
        [SerializeField] private TMP_Text playerBlockText;
        [SerializeField] private CardView cardPrefab;

        [Header("Battle Result UI")]
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private TMP_Text victoryText;
        [SerializeField] private float victoryDelay = 0.8f;
        [SerializeField] private BattleEffectsView battleEffectsView;
        [SerializeField] private BattleStatusView battleStatusView;

        [Header("Card Result UI")]
        [SerializeField] private TMP_Text cardResultText;
        [SerializeField] private GameObject cardResultPanel;
        [SerializeField] private HandLayoutController handLayoutController;

        [Header("Reward UI")]
        [SerializeField] private GameObject rewardPanel;
        [SerializeField] private Transform rewardCardsRoot;
        [SerializeField] private RewardItemView rewardItemPrefab;
        [SerializeField] private Button skipRewardButton;
        [SerializeField] private List<CardSO> rewardPool;
        [SerializeField] private int rewardCardsCount = 3;

        [Header("Battle Config")]
        [SerializeField] private List<CardSO> startingDeck;
        [SerializeField] private string enemyName = "Баба-Яга";
        [SerializeField] private int enemyHp = 40;
        [SerializeField] private int enemyDamage = 6;
        [SerializeField] private TMP_Text enemyIntentText;
        [SerializeField] private TMP_Text enemyBlockText;

        [Header("Reward Icons")]
        [SerializeField] private Sprite goldRewardIcon;
        [SerializeField] private Sprite healRewardIcon;
        
        [Header("Card Play Animation")]
        [SerializeField] private RectTransform playTarget;
        [SerializeField] private RectTransform discardTarget;
        [SerializeField] private float playMoveDuration = 0.22f;
        [SerializeField] private float discardMoveDuration = 0.28f;
        [SerializeField] private float discardScale = 0.25f;
        [SerializeField] private float discardStagger = 0.05f;

        [Header("Scene Transition")]
        [SerializeField] private Button continueButton;
        [SerializeField] private string mapSceneName = "MapScene";
        [SerializeField] private string battleSceneName = "BattleScene";

        private bool _isCardAnimating;
        private bool _rewardShown;

        private IBattleService _battleService;
        private ICardService _cardService;
        private PlayerState _playerState;
        private IObjectResolver _resolver;
        private IMapService _mapService;
        private RunState _runState;
        private RewardService _rewardService;

        private Coroutine _finishRoutine;
        private Coroutine _cardResultRoutine;

        [Inject]
        public void Construct(IBattleService battleService, ICardService cardService, PlayerState playerState, IObjectResolver resolver, IMapService mapService, RunState runState,
            RewardService rewardService)
        {
            _battleService = battleService;
            _cardService = cardService;
            _playerState = playerState;
            _resolver = resolver;
            _mapService = mapService;
            _runState = runState;
            _rewardService = rewardService;
        }

        private void Start()
        {
            if (victoryPanel != null)
                victoryPanel.SetActive(false);

            if (rewardPanel != null)
                rewardPanel.SetActive(false);

            if (cardResultPanel != null)
                cardResultPanel.SetActive(false);

            if (_battleService == null || _cardService == null || _playerState == null || _resolver == null)
            {
                Debug.LogError("BattleScreen: dependencies were not injected.");
                return;
            }

            if (startingDeck == null || startingDeck.Count == 0)
            {
                Debug.LogError("BattleScreen: startingDeck is empty.");
                return;
            }

            if (cardPrefab == null)
            {
                Debug.LogError("BattleScreen: cardPrefab is not assigned.");
                return;
            }

            if (handRoot == null)
            {
                Debug.LogError("BattleScreen: handRoot is not assigned.");
                return;
            }

            if (continueButton != null)
                continueButton.onClick.AddListener(OnContinueClicked);

            if (skipRewardButton != null)
                skipRewardButton.onClick.AddListener(OnSkipRewardClicked);

            if (endTurnButton != null)
                endTurnButton.onClick.AddListener(OnEndTurnClicked);

            _battleService.OnBattleStateChanged += RefreshUI;
            _battleService.OnCardPlayed += OnCardPlayed;
            
            if (battleEffectsView != null)
            {
                _battleService.OnEnemyDamaged += battleEffectsView.ShowEnemyDamage;
                _battleService.OnPlayerDamaged += battleEffectsView.ShowPlayerDamage;
                _battleService.OnPlayerBlocked += battleEffectsView.ShowPlayerBlock;
                _battleService.OnEnemyPoisonTick += battleEffectsView.ShowEnemyPoison;
            }

            if (_playerState.Deck.Count == 0)
                _playerState.Deck.AddRange(startingDeck);
            
            _cardService.InitializeDeck(new List<CardSO>(_playerState.Deck));

            _battleService.StartBattle(new EnemyState
            {
                Name = enemyName,
                HP = enemyHp,
                MaxHP = enemyHp,
                Damage = enemyDamage,
                Weak = 0,
                Poison = 0
            });
        }

        private void OnDestroy()
        {
            StopCoroutineSafe(ref _finishRoutine);
            StopCoroutineSafe(ref _cardResultRoutine);

            if (endTurnButton != null)
                endTurnButton.onClick.RemoveListener(OnEndTurnClicked);

            if (continueButton != null)
                continueButton.onClick.RemoveListener(OnContinueClicked);

            if (skipRewardButton != null)
                skipRewardButton.onClick.RemoveListener(OnSkipRewardClicked);

            if (_battleService != null)
            {
                _battleService.OnBattleStateChanged -= RefreshUI;
                _battleService.OnCardPlayed -= OnCardPlayed;
                
                if (battleEffectsView != null)
                {
                    _battleService.OnEnemyDamaged -= battleEffectsView.ShowEnemyDamage;
                    _battleService.OnPlayerDamaged -= battleEffectsView.ShowPlayerDamage;
                    _battleService.OnPlayerBlocked -= battleEffectsView.ShowPlayerBlock;
                    _battleService.OnEnemyPoisonTick -= battleEffectsView.ShowEnemyPoison;
                }
            }
        }

        private void OnEndTurnClicked()
        {
            if (_isCardAnimating)
                return;

            StartCoroutine(EndTurnRoutine());
        }

        private IEnumerator EndTurnRoutine()
        {
            _isCardAnimating = true;

            yield return DiscardVisibleHandRoutine();

            _battleService.EndPlayerTurn();

            _isCardAnimating = false;

            RefreshUI();
        }

        private IEnumerator DiscardVisibleHandRoutine()
        {
            if (handRoot == null || discardTarget == null)
                yield break;

            int count = handRoot.childCount;

            for (int i = count - 1; i >= 0; i--)
            {
                RectTransform cardRect = handRoot.GetChild(i) as RectTransform;
                if (cardRect == null)
                    continue;

                cardRect.DOKill();
                cardRect.SetAsLastSibling();

                float delay = (count - 1 - i) * discardStagger;

                cardRect.DOMove(discardTarget.position, discardMoveDuration)
                    .SetEase(Ease.InCubic)
                    .SetDelay(delay);

                cardRect.DOScale(Vector3.one * discardScale, discardMoveDuration)
                    .SetEase(Ease.InCubic)
                    .SetDelay(delay);
            }

            yield return new WaitForSeconds(discardMoveDuration + count * discardStagger);
        }

        private void OnCardPlayed(CardSO card, PlayedCardResult result)
        {
            if (cardResultPanel != null)
                cardResultPanel.SetActive(true);
            
            if (cardResultText != null)
            {
                string normalEffectsText = BuildEffectsText(card.Effects);
                string finalEffectsText = BuildEffectsText(result.FinalEffects);

                if (result.WasCorrupted)
                {
                    cardResultText.color = new Color(1f, 0.35f, 0.35f, 1f);
                    cardResultText.text =
                        $"{card.CardName}\n" +
                        $"ИСКАЖЕНО\n" +
                        $"{normalEffectsText} → {finalEffectsText}";
                }
                else
                {
                    cardResultText.color = Color.white;
                    cardResultText.text =
                        $"{card.CardName}\n" +
                        $"{finalEffectsText}";
                }
            }

            StopCoroutineSafe(ref _cardResultRoutine);
            _cardResultRoutine = StartCoroutine(HideCardResultRoutine());
        }

        public void RefreshUI()
        {
            RefreshStats();

            if (!_isCardAnimating)
                RefreshHand();

            RefreshButtons();
            HandleBattleFinishUI();
        }

        private IEnumerator HideCardResultRoutine()
        {
            yield return new WaitForSeconds(1.5f);

            if (cardResultPanel != null)
                cardResultPanel.SetActive(false);

            _cardResultRoutine = null;
        }

        private void RefreshStats()
        {
            if (playerHpText != null)
                playerHpText.text = $"{_playerState.HP}/{_playerState.MaxHP}";

            if (playerEnergyText != null)
                playerEnergyText.text = _playerState.Energy.ToString();

            if (playerBlockText != null)
                playerBlockText.text = _playerState.Block.ToString();

            EnemyState enemy = _battleService.CurrentEnemy;

            if (enemy != null)
            {
                if (enemyHpText != null)
                    enemyHpText.text = $"HP: {enemy.HP}/{enemy.MaxHP}";

                if (enemyBlockText != null)
                    enemyBlockText.text = enemy.Block.ToString();

                if (enemyIntentText != null)
                    enemyIntentText.text = BuildEnemyIntentText(enemy);
            }

            if (battleStatusView != null)
                battleStatusView.Refresh(_playerState, enemy);
        }
        
        private string BuildEnemyIntentText(EnemyState enemy)
        {
            return enemy.IntentType switch
            {
                EnemyIntentType.Attack => $"Намерение: атака {enemy.IntentValue}",
                EnemyIntentType.Block => $"Намерение: защита {enemy.IntentValue}",
                EnemyIntentType.Buff => "Намерение: усиление",
                _ => "Намерение: неизвестно"
            };
        }

        private void RefreshHand()
        {
            if (handRoot == null)
                return;

            IReadOnlyList<CardSO> hand = _cardService.Hand;

            for (int i = handRoot.childCount - 1; i >= hand.Count; i--)
            {
                Transform child = handRoot.GetChild(i);

                if (handLayoutController != null && child is RectTransform rect)
                    handLayoutController.ForgetCard(rect);

                child.SetParent(null);
                Destroy(child.gameObject);
            }

            for (int i = 0; i < hand.Count; i++)
            {
                CardSO card = hand[i];
                if (card == null)
                    continue;

                CardView cardView;

                if (i < handRoot.childCount)
                {
                    cardView = handRoot.GetChild(i).GetComponent<CardView>();
                }
                else
                {
                    cardView = _resolver.Instantiate(cardPrefab, handRoot);
                }

                cardView.Setup(card, false, OnCardViewClicked);
            }

            if (handLayoutController != null)
                handLayoutController.Rebuild();
        }

        private void RefreshButtons()
        {
            if (endTurnButton != null)
                endTurnButton.interactable = !_battleService.IsBattleFinished && !_isCardAnimating;

            if (continueButton != null)
                continueButton.interactable = _battleService.IsBattleFinished;
        }

        private void HandleBattleFinishUI()
        {
            if (!_battleService.IsBattleFinished)
            {
                if (victoryPanel != null)
                    victoryPanel.SetActive(false);

                return;
            }

            if (_finishRoutine != null)
                return;

            _finishRoutine = StartCoroutine(ShowFinishPanelRoutine());
        }

        private IEnumerator ShowFinishPanelRoutine()
        {
            yield return new WaitForSeconds(victoryDelay);

            if (victoryPanel != null)
                victoryPanel.SetActive(true);

            if (victoryText != null)
                victoryText.text = _battleService.PlayerWon ? "ПОБЕДА" : "ПОРАЖЕНИЕ";

            _finishRoutine = null;
        }

        private void OnCardViewClicked(CardView cardView, CardSO card)
        {
            if (_isCardAnimating)
                return;

            if (_battleService == null || _battleService.IsBattleFinished)
                return;

            if (cardView == null || card == null)
                return;

            if (_playerState.Energy < card.Cost)
            {
                ShowNotEnoughEnergy(cardView);
                return;
            }

            StartCoroutine(PlayCardWithDiscardAnimation(cardView, card));
        }

        private void ShowNotEnoughEnergy(CardView cardView)
        {
            RectTransform rect = cardView.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.DOKill();
                rect.DOShakeAnchorPos(0.25f, new Vector2(18f, 0f), 12, 90f);
            }

            if (cardResultPanel != null)
                cardResultPanel.SetActive(true);

            if (cardResultText != null)
            {
                cardResultText.color = new Color(1f, 0.35f, 0.35f, 1f);
                cardResultText.text = "Недостаточно энергии";
            }

            StopCoroutineSafe(ref _cardResultRoutine);
            _cardResultRoutine = StartCoroutine(HideCardResultRoutine());
        }

        private IEnumerator PlayCardWithDiscardAnimation(CardView cardView, CardSO card)
        {
            _isCardAnimating = true;

            RectTransform cardRect = cardView.GetComponent<RectTransform>();

            if (cardRect != null && playTarget != null)
            {
                cardRect.DOKill();
                cardRect.SetAsLastSibling();

                yield return cardRect
                    .DOMove(playTarget.position, playMoveDuration)
                    .SetEase(Ease.OutCubic)
                    .WaitForCompletion();
            }

            _battleService.TryPlayCard(card);

            if (cardRect != null && discardTarget != null)
            {
                cardRect.DOKill();

                Tween moveTween = cardRect
                    .DOMove(discardTarget.position, discardMoveDuration)
                    .SetEase(Ease.InCubic);

                cardRect
                    .DOScale(Vector3.one * discardScale, discardMoveDuration)
                    .SetEase(Ease.InCubic);

                yield return moveTween.WaitForCompletion();
            }

            _isCardAnimating = false;

            RefreshUI();
        }

        private string BuildEffectsText(List<CardEffect> effects)
        {
            if (effects == null || effects.Count == 0)
                return "Без эффекта";

            List<string> parts = new();

            for (int i = 0; i < effects.Count; i++)
            {
                CardEffect effect = effects[i];
                parts.Add($"{GetEffectName(effect.Type)} {effect.Value}");
            }

            return string.Join(", ", parts);
        }

        private string GetEffectName(EffectType type)
        {
            return type switch
            {
                EffectType.Damage => "Урон",
                EffectType.Block => "Блок",
                EffectType.Draw => "Добор",
                EffectType.ApplyWeak => "Слабость",
                EffectType.ApplyPoison => "Яд",
                EffectType.GainEnergy => "Энергия",
                _ => type.ToString()
            };
        }

        private void StopCoroutineSafe(ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        private void OnContinueClicked()
        {
            if (_battleService == null || _runState == null)
                return;

            if (!_battleService.PlayerWon)
            {
                RunLifetimeScope.LoadScene(battleSceneName);
                return;
            }

            ShowRewardPanel();
        }

        private void ShowRewardPanel()
        {
            if (_rewardShown)
                return;

            _rewardShown = true;

            if (victoryPanel != null)
                victoryPanel.SetActive(false);

            if (rewardPanel != null)
                rewardPanel.SetActive(true);

            BuildRewardCards();
        }

        private void BuildRewardCards()
        {
            if (rewardCardsRoot == null || rewardItemPrefab == null)
            {
                Debug.LogError("BattleScreen: rewardCardsRoot or rewardItemPrefab is not assigned.");
                return;
            }

            for (int i = rewardCardsRoot.childCount - 1; i >= 0; i--)
                Destroy(rewardCardsRoot.GetChild(i).gameObject);

            if (rewardPool == null || rewardPool.Count == 0)
            {
                Debug.LogError("BattleScreen: rewardPool is empty.");
                return;
            }

            List<RewardItem> rewards = _rewardService != null
                ? _rewardService.GenerateBattleRewards(rewardPool)
                : new List<RewardItem>();

            rewards = AddRewardIcons(rewards);

            if (rewards == null || rewards.Count == 0)
            {
                Debug.LogError("BattleScreen: reward items were not generated.");
                return;
            }

            foreach (RewardItem reward in rewards)
            {
                RewardItemView view = Instantiate(rewardItemPrefab, rewardCardsRoot);
                view.Setup(reward, OnRewardSelected);
            }
        }
        
        private List<RewardItem> AddRewardIcons(List<RewardItem> rewards)
        {
            List<RewardItem> result = new();

            foreach (RewardItem reward in rewards)
            {
                switch (reward.Type)
                {
                    case RewardType.Gold:
                        result.Add(new RewardItem(
                            RewardType.Gold,
                            amount: reward.Amount,
                            icon: goldRewardIcon));
                        break;

                    case RewardType.Heal:
                        result.Add(new RewardItem(
                            RewardType.Heal,
                            amount: reward.Amount,
                            icon: healRewardIcon));
                        break;

                    default:
                        result.Add(reward);
                        break;
                }
            }

            return result;
        }

        private void OnRewardSelected(RewardItem reward)
        {
            if (reward == null)
                return;

            switch (reward.Type)
            {
                case RewardType.Card:
                    if (reward.Card != null)
                        _playerState.Deck.Add(reward.Card);
                    break;

                case RewardType.Gold:
                    _runState.AddGold(reward.Amount);
                    break;

                case RewardType.Heal:
                    _playerState.Heal(reward.Amount);
                    break;
            }

            GoToMapAfterReward();
        }
        
        private void OnSkipRewardClicked()
        {
            _runState.AddSkippedReward();
            GoToMapAfterReward();
        }

        private void GoToMapAfterReward()
        {
            if (_mapService != null && _mapService.State != null && _mapService.State.SelectedNodeId >= 0)
            {
                _mapService.CompleteSelectedNode();
            }
            else
            {
                _runState.IntroBattleCompleted = true;
            }

            RunLifetimeScope.LoadScene(mapSceneName);
        }
    }
}