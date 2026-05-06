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
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class BattleScreen : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Button endTurnButton;
        [SerializeField] private HandView handView;
        [SerializeField] private BattleHudView battleHudView;

        [Header("Battle Result UI")]
        [SerializeField] private BattleResultView battleResultView;
        [SerializeField] private float victoryDelay = 0.8f;
        [SerializeField] private BattleEffectsView battleEffectsView;
        [SerializeField] private BattleStatusView battleStatusView;

        [Header("Card Result UI")]
        [SerializeField] private CardResultView cardResultView;

        [Header("Reward UI")]
        [SerializeField] private BattleRewardView battleRewardView;

        [Header("Battle Config")]
        [SerializeField] private List<CardSO> startingDeck;
        [SerializeField] private EnemySO enemyConfig;
        
        [Header("Card Play Animation")]
        [SerializeField] private CardPlayAnimator cardPlayAnimator;

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
            if (battleResultView != null)
                battleResultView.Hide();

            if (battleRewardView != null)
                battleRewardView.Initialize(_rewardService, OnRewardSelected, OnSkipRewardClicked);
            
            if (cardResultView != null)
                cardResultView.HideInstant();

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
            
            if (handView != null)
                handView.Initialize(_resolver, OnCardViewClicked);

            if (continueButton != null)
                continueButton.onClick.AddListener(OnContinueClicked);

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

            if (enemyConfig == null)
            {
                Debug.LogError("BattleScreen: enemyConfig is not assigned.");
                return;
            }

            _battleService.StartBattle(enemyConfig.CreateState());
        }

        private void OnDestroy()
        {
            StopCoroutineSafe(ref _finishRoutine);

            if (endTurnButton != null)
                endTurnButton.onClick.RemoveListener(OnEndTurnClicked);

            if (continueButton != null)
                continueButton.onClick.RemoveListener(OnContinueClicked);

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

            if (cardPlayAnimator != null && handView != null)
                yield return cardPlayAnimator.DiscardHand(handView.Root);

            _battleService.EndPlayerTurn();

            _isCardAnimating = false;

            RefreshUI();
        }

        private void OnCardPlayed(CardSO card, PlayedCardResult result)
        {
            if (cardResultView != null)
                cardResultView.ShowCardResult(card, result);
        }

        public void RefreshUI()
        {
            RefreshStats();

            if (!_isCardAnimating && handView != null)
                handView.Refresh(_cardService.Hand);

            RefreshButtons();
            HandleBattleFinishUI();
        }

        private void RefreshStats()
        {
            EnemyState enemy = _battleService.CurrentEnemy;

            if (battleHudView != null)
                battleHudView.Refresh(_playerState, enemy);

            if (battleStatusView != null)
                battleStatusView.Refresh(_playerState, enemy);
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
                if (battleResultView != null)
                    battleResultView.Hide();

                return;
            }

            if (_finishRoutine != null)
                return;

            _finishRoutine = StartCoroutine(ShowFinishPanelRoutine());
        }

        private IEnumerator ShowFinishPanelRoutine()
        {
            yield return new WaitForSeconds(victoryDelay);

            if (battleResultView != null)
                battleResultView.Show(_battleService.PlayerWon);

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

            if (cardResultView != null)
                cardResultView.ShowNotEnoughEnergy();
        }

        private IEnumerator PlayCardWithDiscardAnimation(CardView cardView, CardSO card)
        {
            _isCardAnimating = true;

            if (cardPlayAnimator != null)
                yield return cardPlayAnimator.MoveToPlay(cardView);

            bool played = _battleService.TryPlayCard(card);

            if (played && cardPlayAnimator != null)
                yield return cardPlayAnimator.MoveToDiscard(cardView);

            _isCardAnimating = false;
            RefreshUI();
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

            if (battleResultView != null)
                battleResultView.Hide();

            if (battleRewardView != null)
                battleRewardView.Show();
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