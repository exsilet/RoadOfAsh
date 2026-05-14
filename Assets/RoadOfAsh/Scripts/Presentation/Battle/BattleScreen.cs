using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain;
using RoadOfAsh.Scripts.Domain.Battle;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Distortion;
using RoadOfAsh.Scripts.Domain.Map;
using RoadOfAsh.Scripts.Domain.Players;
using RoadOfAsh.Scripts.Domain.Relics;
using RoadOfAsh.Scripts.Domain.Rewards;
using RoadOfAsh.Scripts.Presentation.Tutorial;
using UnityEngine;
using VContainer;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class BattleScreen : MonoBehaviour
    {
        [Header("Hand Flow")] [SerializeField] private BattleHandFlow battleHandFlow;

        [Header("UI Refresh Flow")] [SerializeField]
        private BattleUiRefreshFlow battleUiRefreshFlow;

        [Header("Setup Flow")] [SerializeField]
        private BattleSetupFlow battleSetupFlow;

        [Header("Card Play Flow")] [SerializeField]
        private BattleCardPlayFlow battleCardPlayFlow;

        [Header("Turn Flow")] [SerializeField] private BattleTurnFlow battleTurnFlow;

        [Header("Tutorial")] [SerializeField] private TutorialBattleFlow tutorialBattleFlow;

        [Header("Feedback Flow")] [SerializeField]
        private BattleFeedbackFlow battleFeedbackFlow;

        [Header("Reward Flow")] [SerializeField]
        private BattleRewardFlow battleRewardFlow;

        [Header("Result Flow")] [SerializeField]
        private BattleResultFlow battleResultFlow;

        [Header("Aftermath Flow")] [SerializeField]
        private BattleAftermathFlow battleAftermathFlow;

        [Header("Scene Flow")] [SerializeField]
        private BattleSceneFlow battleSceneFlow;

        private IBattleService _battleService;
        private ICardService _cardService;
        private PlayerState _playerState;
        private IObjectResolver _resolver;
        private IMapService _mapService;
        private RunState _runState;
        private IRewardService _rewardService;
        private IDistortionService _distortionService;
        private IRelicService _relicService;

        [Inject]
        public void Construct(IBattleService battleService, ICardService cardService, PlayerState playerState,
            IObjectResolver resolver, IMapService mapService,
            RunState runState, IRewardService rewardService, IDistortionService distortionService,
            IRelicService relicService)
        {
            _battleService = battleService;
            _cardService = cardService;
            _playerState = playerState;
            _resolver = resolver;
            _mapService = mapService;
            _runState = runState;
            _rewardService = rewardService;
            _relicService = relicService;
            _distortionService = distortionService;
        }

        private void Start()
        {
            if (_battleService == null || _cardService == null || _playerState == null || _resolver == null || _mapService == null ||
                _runState == null || _rewardService == null || _distortionService == null || _relicService == null)
            {
                Debug.LogError("BattleScreen: dependencies were not injected.");
                return;
            }

            if (battleRewardFlow != null)
            {
                battleRewardFlow.Initialize(_playerState, _runState, _relicService, _rewardService);
                battleRewardFlow.Completed += OnRewardFlowCompleted;
            }

            if (battleHandFlow != null)
                battleHandFlow.Initialize(_resolver);

            if (battleResultFlow != null)
            {
                battleResultFlow.Initialize();
                battleResultFlow.ContinueClicked += OnContinueClicked;
                battleResultFlow.RestartRunClicked += OnRestartRunClicked;
            }

            if (battleSceneFlow != null)
                battleSceneFlow.Initialize(_playerState, _cardService, _mapService, _runState, _distortionService);

            if (battleSetupFlow != null)
                battleSetupFlow.Initialize(_battleService, _cardService, _playerState, _mapService, _runState,
                    _distortionService);

            if (battleFeedbackFlow != null)
                battleFeedbackFlow.Initialize(_battleService, _relicService);

            if (battleCardPlayFlow != null)
            {
                battleCardPlayFlow.Initialize(_battleService, _playerState);
                battleCardPlayFlow.CardPlayFinished += RefreshUI;
            }

            if (battleTurnFlow != null)
            {
                battleTurnFlow.Initialize(_battleService, IsCardAnimating);
                battleTurnFlow.TurnEnded += RefreshUI;
            }

            if (battleUiRefreshFlow != null)
                battleUiRefreshFlow.Initialize(_battleService, _cardService, _playerState, _distortionService, IsBusy);

            _battleService.OnBattleStateChanged += RefreshUI;

            if (battleAftermathFlow != null)
                battleAftermathFlow.CacheRelicsAtBattleStart();

            if (battleSetupFlow == null || !battleSetupFlow.TryStartBattle())
                return;

            if (battleUiRefreshFlow != null)
                battleUiRefreshFlow.SetUnderstandingBeforeBattle(battleSetupFlow.UnderstandingBeforeBattle);

            RefreshUI();
        }

        private void OnDestroy()
        {
            if (battleRewardFlow != null)
                battleRewardFlow.Completed -= OnRewardFlowCompleted;

            if (battleResultFlow != null)
            {
                battleResultFlow.ContinueClicked -= OnContinueClicked;
                battleResultFlow.RestartRunClicked -= OnRestartRunClicked;
                battleResultFlow.Dispose();
            }

            if (_battleService != null)
            {
                _battleService.OnBattleStateChanged -= RefreshUI;
            }

            if (battleCardPlayFlow != null)
                battleCardPlayFlow.CardPlayFinished -= RefreshUI;

            if (battleTurnFlow != null)
            {
                battleTurnFlow.TurnEnded -= RefreshUI;
                battleTurnFlow.Dispose();
            }

            if (battleFeedbackFlow != null)
                battleFeedbackFlow.Dispose();
        }

        public void RefreshUI()
        {
            if (battleUiRefreshFlow != null)
                battleUiRefreshFlow.Refresh();
        }

        private bool IsCardAnimating()
        {
            return battleCardPlayFlow != null && battleCardPlayFlow.IsCardAnimating;
        }

        private bool IsBusy()
        {
            return IsCardAnimating() || (battleTurnFlow != null && battleTurnFlow.IsEndingTurn);
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
                if (battleRewardFlow != null && battleRewardFlow.CanShowTutorialReward())
                {
                    if (battleResultFlow != null)
                        battleResultFlow.Hide();

                    battleRewardFlow.ShowTutorialReward();
                }
                else
                {
                    if (battleSceneFlow != null)
                        battleSceneFlow.CompleteTutorialAndGoToMap();
                }

                return;
            }

            if (IsSelectedBossNode())
            {
                if (battleResultFlow != null)
                    battleResultFlow.ShowChapterComplete();

                return;
            }

            ShowRewardPanel();
        }

        private void RestartAfterDefeat()
        {
            if (battleSetupFlow == null || battleSceneFlow == null)
                return;

            bool isTutorialBattle = battleSetupFlow.IsTutorialBattle;
            List<CardSO> deckForBattle = battleSetupFlow.BuildRestartDeck();

            battleSceneFlow.RestartBattleAfterDefeat(battleSetupFlow.GetStartingDeck(), deckForBattle,
                isTutorialBattle);
        }

        private void ShowRewardPanel()
        {
            if (battleResultFlow != null)
                battleResultFlow.Hide();

            if (battleRewardFlow != null)
                battleRewardFlow.ShowRegularReward();
            else
                GoToMapAfterReward();
        }

        private void OnRewardFlowCompleted()
        {
            GoToMapAfterReward();
        }

        private void GoToMapAfterReward()
        {
            if (battleAftermathFlow != null)
                battleAftermathFlow.ApplyAfterBattleRelics(_battleService.PlayerWon);

            if (battleSceneFlow != null)
                battleSceneFlow.GoToMapAfterReward();
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
            if (battleSceneFlow != null)
                battleSceneFlow.GoToMainMenu();
        }
    }
}