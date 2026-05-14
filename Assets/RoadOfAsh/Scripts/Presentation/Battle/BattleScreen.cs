using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RoadOfAsh.Scripts.Domain;
using RoadOfAsh.Scripts.Domain.Battle;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Distortion;
using RoadOfAsh.Scripts.Domain.Map;
using RoadOfAsh.Scripts.Domain.Players;
using RoadOfAsh.Scripts.Domain.Rewards;
using RoadOfAsh.Scripts.Infrastructure;
using RoadOfAsh.Scripts.Presentation.Rewards;
using RoadOfAsh.Scripts.Presentation.Tutorial;
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
        
        [SerializeField] private UnderstandingView understandingView;
        [SerializeField] private StatusTooltipSystem tooltipSystem;

        [Header("Battle Result UI")]
        [SerializeField] private BattleCompletionView battleCompletionView;
        [SerializeField] private BattleEffectsView battleEffectsView;
        [SerializeField] private BattleStatusView battleStatusView;

        [Header("Card Result UI")]
        [SerializeField] private CardResultView cardResultView;

        [Header("Reward UI")]
        [SerializeField] private RewardSelectionView rewardSelectionView;
        
        [Header("Tutorial")]
        [SerializeField] private TutorialBattleFlow tutorialBattleFlow;

        [Header("Battle Config")]
        [SerializeField] private StarterDeckSO startingDeck;
        [SerializeField] private EnemySO tutorialEnemy;
        [SerializeField] private EnemySO fallbackEnemyConfig;
        
        [Header("Tutorial Cards")]
        [SerializeField] private CardSO tutorialAttackCard;
        [SerializeField] private CardSO tutorialBlockCard;
        [SerializeField] private List<CardSO> tutorialExtraCards = new();
        
        [Header("Tutorial Reward")]
        [SerializeField] private bool showTutorialReward = true;
        [SerializeField] private CardSO tutorialRewardCard;
        
        [Header("Card Play Animation")]
        [SerializeField] private CardPlayAnimator cardPlayAnimator;

        [Header("Scene Transition")]
        [SerializeField] private string mainMenuSceneName = "MainMenu";
        [SerializeField] private string mapSceneName = "MapScene";
        [SerializeField] private string battleSceneName = "BattleScene";
        
        private int _understandingBeforeBattle;
        
        private bool _isCardAnimating;
        private bool _rewardShown;
        private bool _finishShown;

        private IBattleService _battleService;
        private ICardService _cardService;
        private PlayerState _playerState;
        private IObjectResolver _resolver;
        private IMapService _mapService;
        private RunState _runState;
        private IRewardService _rewardService;
        private IDistortionService _distortionService;

        [Inject]
        public void Construct(IBattleService battleService, ICardService cardService, PlayerState playerState, IObjectResolver resolver, IMapService mapService, RunState runState,
            IRewardService rewardService, IDistortionService distortionService)
        {
            _battleService = battleService;
            _cardService = cardService;
            _playerState = playerState;
            _resolver = resolver;
            _mapService = mapService;
            _runState = runState;
            _rewardService = rewardService;
            _distortionService = distortionService;
        }

        private void Start()
        {
            if (rewardSelectionView != null)
                rewardSelectionView.Initialize(_rewardService, OnRewardSelected, OnSkipRewardClicked);
            
            if (cardResultView != null)
                cardResultView.HideInstant();

            if (_battleService == null || _cardService == null || _playerState == null || _resolver == null)
            {
                Debug.LogError("BattleScreen: dependencies were not injected.");
                return;
            }

            if (startingDeck == null || startingDeck.Cards == null || startingDeck.Cards.Count == 0)
            {
                Debug.LogError("BattleScreen: startingDeck is empty.");
                return;
            }
            
            if (handView != null)
                handView.Initialize(_resolver, OnCardViewClicked);
            
            if (battleCompletionView != null)
            {
                battleCompletionView.HideAll();
                battleCompletionView.ContinueClicked += OnContinueClicked;
                battleCompletionView.RestartRunClicked += OnRestartRunClicked;
            }

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
                
                _battleService.OnPlayerPoisoned += battleEffectsView.ShowPlayerPoison;
                _battleService.OnPlayerWeakened += battleEffectsView.ShowPlayerWeak;
                _battleService.OnEnemyHealed += battleEffectsView.ShowEnemyHeal;
                _battleService.OnEnemyCleansed += battleEffectsView.ShowEnemyCleanse;
            }

            bool isTutorialBattle = _runState != null && !_runState.IntroBattleCompleted;

            List<CardSO> deckForBattle = isTutorialBattle ? BuildTutorialDeck() : BuildRegularBattleDeck();

            _playerState.Deck.Clear();
            _playerState.Deck.AddRange(deckForBattle);

            _cardService.InitializeDeck(new List<CardSO>(_playerState.Deck), !isTutorialBattle);

            EnemySO enemy = GetEnemyForBattle();

            if (enemy == null)
            {
                Debug.LogError("BattleScreen: enemy config is not assigned.");
                return;
            }
            
            _understandingBeforeBattle = _distortionService?.Understanding ?? 0;

            _battleService.StartBattle(enemy.CreateState());
        }

        private void OnDestroy()
        {
            if (endTurnButton != null)
                endTurnButton.onClick.RemoveListener(OnEndTurnClicked);

            if (battleCompletionView != null)
            {
                battleCompletionView.ContinueClicked -= OnContinueClicked;
                battleCompletionView.RestartRunClicked -= OnRestartRunClicked;
            }

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
                    
                    _battleService.OnPlayerPoisoned -= battleEffectsView.ShowPlayerPoison;
                    _battleService.OnPlayerWeakened -= battleEffectsView.ShowPlayerWeak;
                    _battleService.OnEnemyHealed -= battleEffectsView.ShowEnemyHeal;
                    _battleService.OnEnemyCleansed -= battleEffectsView.ShowEnemyCleanse;
                }
            }
        }
        
        private List<CardSO> BuildRegularBattleDeck()
        {
            if (_playerState.Deck.Count > 0)
                return new List<CardSO>(_playerState.Deck);

            return new List<CardSO>(startingDeck.Cards);
        }
        
        private EnemySO GetEnemyForBattle()
        {
            if (_runState != null && !_runState.IntroBattleCompleted)
                return tutorialEnemy;

            if (_mapService != null && _mapService.State != null)
            {
                MapNodeData selectedNode = _mapService.GetSelectedNode();

                if (selectedNode != null && selectedNode.Enemy != null)
                    return selectedNode.Enemy;
            }

            return fallbackEnemyConfig;
        }
        
        private List<CardSO> BuildTutorialDeck()
        {
            List<CardSO> result = new();

            if (tutorialAttackCard != null)
                result.Add(tutorialAttackCard);

            if (tutorialBlockCard != null)
                result.Add(tutorialBlockCard);

            foreach (CardSO card in tutorialExtraCards)
            {
                if (card == null)
                    continue;

                if (result.Contains(card))
                    continue;

                result.Add(card);
            }

            foreach (CardSO card in startingDeck.Cards)
            {
                if (card == null)
                    continue;

                if (result.Contains(card))
                    continue;

                result.Add(card);
            }

            return result;
        }

        private void OnEndTurnClicked()
        {
            if (_isCardAnimating)
                return;

            if (tutorialBattleFlow != null && !tutorialBattleFlow.CanEndTurn())
            {
                ShakeEndTurnButton();
                return;
            }

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

            if (understandingView != null && _distortionService != null)
                understandingView.Refresh(_distortionService.Understanding, DistortionService.MaxUnderstanding);
        }

        private void RefreshButtons()
        {
            if (endTurnButton != null)
            {
                bool canEndTurn = !_battleService.IsBattleFinished && !_isCardAnimating && (tutorialBattleFlow == null || tutorialBattleFlow.CanEndTurn());
                endTurnButton.interactable = canEndTurn;
            }

            if (battleCompletionView != null)
                battleCompletionView.SetContinueInteractable(_battleService.IsBattleFinished);
        }

        private void HandleBattleFinishUI()
        {
            if (!_battleService.IsBattleFinished)
            {
                _finishShown = false;

                if (battleCompletionView != null)
                    battleCompletionView.HideAll();

                return;
            }

            if (_finishShown)
                return;

            _finishShown = true;
            
            if (_battleService.PlayerWon && understandingView != null && _distortionService != null) 
                understandingView.PlayGain(_understandingBeforeBattle, _distortionService.Understanding, DistortionService.MaxUnderstanding);

            if (battleCompletionView != null)
                battleCompletionView.ShowBattleResult(_battleService.PlayerWon);
        }

        private void OnCardViewClicked(CardView cardView, CardSO card)
        {
            if (_isCardAnimating)
                return;

            if (_battleService == null || _battleService.IsBattleFinished)
                return;

            if (cardView == null || card == null)
                return;

            if (tutorialBattleFlow != null && !tutorialBattleFlow.CanPlayCard(card))
            {
                ShowWrongTutorialCard(cardView);
                return;
            }

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
        
        private void ShowWrongTutorialCard(CardView cardView)
        {
            RectTransform rect = cardView.GetComponent<RectTransform>();

            if (rect != null)
            {
                rect.DOKill();
                rect.DOShakeAnchorPos(0.25f, new Vector2(14f, 0f), 10, 90f);
            }
        }
        
        private void ShakeEndTurnButton()
        {
            if (endTurnButton == null)
                return;

            RectTransform rect = endTurnButton.GetComponent<RectTransform>();

            if (rect == null)
                return;

            rect.DOKill();
            rect.DOShakeAnchorPos(0.25f, new Vector2(14f, 0f), 10, 90f);
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

        private void OnContinueClicked()
        {
            if (_battleService == null || _runState == null)
                return;

            if (!_battleService.PlayerWon)
            {
                RestartAfterDefeat();
                return;
            }

            if (!_runState.IntroBattleCompleted)
            {
                if (showTutorialReward && tutorialRewardCard != null)
                    ShowTutorialRewardPanel();
                else
                    CompleteTutorialAndGoToMap();

                return;
            }

            if (IsSelectedBossNode())
            {
                if (battleCompletionView != null)
                    battleCompletionView.ShowChapterComplete();

                return;
            }

            ShowRewardPanel();
        }
        
        private void CompleteTutorialAndGoToMap()
        {
            _distortionService.SetRandomDistortionEnabled(true);
            _distortionService.ResetUnderstanding();

            _runState.IntroBattleCompleted = true;
            RunLifetimeScope.LoadScene(mapSceneName);
        }
        
        private void RestartAfterDefeat()
        {
            _playerState.HP = Mathf.Max(1, Mathf.RoundToInt(_playerState.MaxHP * 0.5f));
            _playerState.Block = 0;
            _playerState.Energy = 3;
            _playerState.Weak = 0;
            _playerState.Poison = 0;

            _playerState.Hand.Clear();
            _playerState.Discard.Clear();

            bool isTutorialBattle = _runState != null && !_runState.IntroBattleCompleted;

            List<CardSO> deckForBattle = isTutorialBattle ? BuildTutorialDeck() : new List<CardSO>(startingDeck.Cards);

            _playerState.Deck.Clear();
            _playerState.Deck.AddRange(deckForBattle);

            _cardService.InitializeDeck(new List<CardSO>(_playerState.Deck), !isTutorialBattle);

            RunLifetimeScope.LoadScene(battleSceneName);
        }

        private void ShowRewardPanel()
        {
            if (_rewardShown)
                return;

            _rewardShown = true;

            if (battleCompletionView != null)
                battleCompletionView.HideAll();

            if (rewardSelectionView != null)
                rewardSelectionView.Show();
        }
        
        private void ShowTutorialRewardPanel()
        {
            if (_rewardShown)
                return;

            _rewardShown = true;

            if (battleCompletionView != null)
                battleCompletionView.HideAll();

            if (rewardSelectionView == null)
            {
                CompleteTutorialAndGoToMap();
                return;
            }

            List<RewardItem> rewards = new()
            {
                new RewardItem(RewardType.Card, tutorialRewardCard)
            };

            rewardSelectionView.ShowFixed(rewards);
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
                _distortionService.SetRandomDistortionEnabled(true);
                _distortionService.ResetUnderstanding();
                _runState.IntroBattleCompleted = true;
            }

            RunLifetimeScope.LoadScene(mapSceneName);
        }
        
        private bool IsSelectedBossNode()
        {
            if (_mapService == null || _mapService.State == null)
                return false;

            MapNodeData selectedNode = _mapService.GetSelectedNode();

            return selectedNode != null && selectedNode.Type == MapNodeType.Boss;
        }
        
        private void OnRestartRunClicked()
        {
            RunLifetimeScope.LoadScene(mainMenuSceneName);
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            ValidateTutorialCards();
        }

        private void ValidateTutorialCards()
        {
            if (tutorialAttackCard != null && tutorialAttackCard.CanAppearInRewards)
            {
                Debug.LogWarning(
                    $"BattleScreen: tutorialAttackCard '{tutorialAttackCard.CardName}' выглядит как reward-карта. " +
                    "Для обучения лучше использовать стартовую карту атаки.",
                    this);
            }

            if (tutorialBlockCard != null && tutorialBlockCard.CanAppearInRewards)
            {
                Debug.LogWarning(
                    $"BattleScreen: tutorialBlockCard '{tutorialBlockCard.CardName}' выглядит как reward-карта. " +
                    "Для обучения лучше использовать стартовую карту блока.",
                    this);
            }

            if (tutorialExtraCards == null)
                return;

            foreach (CardSO card in tutorialExtraCards)
            {
                if (card == null)
                    continue;

                if (card.CanAppearInRewards)
                {
                    Debug.LogWarning(
                        $"BattleScreen: tutorialExtraCards содержит reward-карту '{card.CardName}'. " +
                        "В обучении лучше использовать только стартовые карты.",
                        this);
                }
            }
        }
#endif
    }
}