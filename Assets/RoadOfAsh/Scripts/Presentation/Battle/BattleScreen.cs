using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain;
using RoadOfAsh.Scripts.Domain.Battle;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Distortion;
using RoadOfAsh.Scripts.Domain.Map;
using RoadOfAsh.Scripts.Domain.Players;
using RoadOfAsh.Scripts.Domain.Relics;
using RoadOfAsh.Scripts.Domain.Rewards;
using RoadOfAsh.Scripts.Infrastructure.Ads;
using RoadOfAsh.Scripts.Infrastructure.Saves;
using RoadOfAsh.Scripts.Presentation.Tutorial;
using UnityEngine;
using VContainer;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class BattleScreen : MonoBehaviour
    {
        [Header("Hand Flow")]
        [SerializeField] private BattleHandFlow battleHandFlow;

        [Header("UI Refresh Flow")]
        [SerializeField] private BattleUiRefreshFlow battleUiRefreshFlow;

        [Header("Setup Flow")]
        [SerializeField] private BattleSetupFlow battleSetupFlow;

        [Header("Card Play Flow")]
        [SerializeField] private BattleCardPlayFlow battleCardPlayFlow;

        [Header("Turn Flow")]
        [SerializeField] private BattleTurnFlow battleTurnFlow;

        [Header("Tutorial")]
        [SerializeField] private TutorialBattleFlow tutorialBattleFlow;

        [Header("Feedback Flow")]
        [SerializeField] private BattleFeedbackFlow battleFeedbackFlow;

        [Header("Reward Flow")]
        [SerializeField] private BattleRewardFlow battleRewardFlow;

        [Header("Result Flow")]
        [SerializeField] private BattleResultFlow battleResultFlow;

        [Header("Aftermath Flow")]
        [SerializeField] private BattleAftermathFlow battleAftermathFlow;

        [Header("Scene Flow")]
        [SerializeField] private BattleSceneFlow battleSceneFlow;

        private IBattleService _battleService;
        private ICardService _cardService;
        private PlayerState _playerState;
        private IObjectResolver _resolver;
        private IMapService _mapService;
        private RunState _runState;
        private IRewardService _rewardService;
        private IDistortionService _distortionService;
        private IRelicService _relicService;
        private IAdService _adService;
        private ISaveService _saveService;

        private bool _reviveUsed;
        private bool _chapterCompleteShown;

        [Inject]
        public void Construct(IBattleService battleService, ICardService cardService, PlayerState playerState, IObjectResolver resolver, IMapService mapService, RunState runState,
            IRewardService rewardService, IDistortionService distortionService, IRelicService relicService, IAdService adService, ISaveService saveService)
        {
            _battleService = battleService;
            _cardService = cardService;
            _playerState = playerState;
            _resolver = resolver;
            _mapService = mapService;
            _runState = runState;
            _rewardService = rewardService;
            _distortionService = distortionService;
            _relicService = relicService;
            _adService = adService;
            _saveService = saveService;
        }

        private void Start()
        {
            if (_battleService == null || _cardService == null || _playerState == null ||
                _resolver == null || _mapService == null || _runState == null ||
                _rewardService == null || _distortionService == null || _relicService == null ||
                _adService == null || _saveService == null)
            {
                Debug.LogError("BattleScreen: dependencies were not injected.");
                return;
            }

            _chapterCompleteShown = false;
            _reviveUsed = false;

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
                battleResultFlow.FreeReviveClicked += OnFreeReviveClicked;
                battleResultFlow.RewardReviveClicked += OnRewardReviveClicked;
                battleResultFlow.RestartRunClicked += OnRestartRunClicked;
                battleResultFlow.MainMenuClicked += OnMainMenuClicked;
                battleResultFlow.ChapterContinueClicked += OnChapterContinueClicked;
            }

            if (battleSceneFlow != null) 
                battleSceneFlow.Initialize(_playerState, _cardService, _mapService, _runState, _distortionService, _saveService);

            if (battleSetupFlow != null) 
                battleSetupFlow.Initialize(_battleService, _cardService, _playerState, _mapService, _runState, _distortionService);

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
            {
                battleAftermathFlow.Initialize(_playerState, _runState);
                battleAftermathFlow.CacheRelicsAtBattleStart();
            }

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
                battleResultFlow.FreeReviveClicked -= OnFreeReviveClicked;
                battleResultFlow.RewardReviveClicked -= OnRewardReviveClicked;
                battleResultFlow.RestartRunClicked -= OnRestartRunClicked;
                battleResultFlow.MainMenuClicked -= OnMainMenuClicked;
                battleResultFlow.ChapterContinueClicked -= OnChapterContinueClicked;
                battleResultFlow.Dispose();
            }

            if (_battleService != null)
                _battleService.OnBattleStateChanged -= RefreshUI;

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
                return;

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
                if (!_chapterCompleteShown)
                {
                    _chapterCompleteShown = true;

                    if (battleResultFlow != null)
                        battleResultFlow.ShowChapterComplete();

                    return;
                }

                if (battleSceneFlow != null)
                    battleSceneFlow.GoToMainMenu();

                return;
            }

            ShowRewardPanel();
        }
        
        private void OnFreeReviveClicked()
        {
            if (_battleService == null || _battleService.PlayerWon || _reviveUsed)
                return;

            _reviveUsed = true;

            if (battleResultFlow != null)
                battleResultFlow.SetDefeatReviveOptionsAvailable(false);

            RevivePlayerWithoutAd();
        }

        private void OnRewardReviveClicked()
        {
            if (_battleService == null || _battleService.PlayerWon || _reviveUsed)
                return;

            if (_adService == null || !_adService.IsRewardedAvailable)
                return;

            if (battleResultFlow != null)
                battleResultFlow.SetRewardReviveInteractable(false);

            _adService.ShowRewarded(rewardId: "revive_after_defeat", onSuccess: RevivePlayerAfterAd, onFailed: OnRewardReviveFailed);
        }

        private void OnRewardReviveFailed()
        {
            if (battleResultFlow != null)
                battleResultFlow.SetRewardReviveInteractable(true);
        }
        
        private void RevivePlayerWithoutAd()
        {
            int reviveHp = Mathf.Max(1, Mathf.RoundToInt(_playerState.MaxHP * 0.5f));

            _battleService.RevivePlayer(reviveHp);

            if (battleResultFlow != null)
                battleResultFlow.Hide();

            RefreshUI();
        }
        
        private void RevivePlayerAfterAd()
        {
            _reviveUsed = true;

            if (battleResultFlow != null)
                battleResultFlow.SetDefeatReviveOptionsAvailable(false);

            int reviveHp = _playerState.MaxHP;

            _battleService.RevivePlayer(reviveHp);

            if (battleResultFlow != null)
            {
                battleResultFlow.Hide();
                battleResultFlow.SetRewardReviveInteractable(true);
            }

            RefreshUI();
        }
        
        private void OnChapterContinueClicked()
        {
            if (battleSceneFlow != null)
                battleSceneFlow.GoToMainMenu();
        }

        private void OnMainMenuClicked()
        {
            if (battleSceneFlow != null)
                battleSceneFlow.GoToMainMenu();
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
            RestartAfterDefeat();
        }
        
        private void RestartAfterDefeat()
        {
            if (battleSetupFlow == null || battleSceneFlow == null)
                return;

            bool isTutorialBattle = battleSetupFlow.IsTutorialBattle;
            List<CardSO> deckForBattle = battleSetupFlow.BuildRestartDeck();

            battleSceneFlow.RestartBattleAfterDefeat(battleSetupFlow.GetStartingDeck(), deckForBattle, isTutorialBattle);
        }
    }
}