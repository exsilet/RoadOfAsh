using System;
using RoadOfAsh.Scripts.Domain.Battle;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Distortion;
using RoadOfAsh.Scripts.Domain.Players;
using RoadOfAsh.Scripts.Presentation.Relics;
using UnityEngine;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class BattleUiRefreshFlow : MonoBehaviour
    {
        [SerializeField] private HandView handView;
        [SerializeField] private BattleHudView battleHudView;
        [SerializeField] private BattleStatusView battleStatusView;
        [SerializeField] private UnderstandingView understandingView;
        [SerializeField] private RelicBarView relicBarView;
        [SerializeField] private BattleResultFlow battleResultFlow;
        [SerializeField] private BattleTurnFlow battleTurnFlow;

        private IBattleService _battleService;
        private ICardService _cardService;
        private PlayerState _playerState;
        private IDistortionService _distortionService;

        private Func<bool> _isBusy;
        private int _understandingBeforeBattle;
        private bool _understandingGainShown;

        public void Initialize(IBattleService battleService, ICardService cardService, PlayerState playerState, IDistortionService distortionService,
            Func<bool> isBusy)
        {
            _battleService = battleService;
            _cardService = cardService;
            _playerState = playerState;
            _distortionService = distortionService;
            _isBusy = isBusy;
        }

        public void SetUnderstandingBeforeBattle(int value)
        {
            _understandingBeforeBattle = value;
            _understandingGainShown = false;
        }

        public void Refresh()
        {
            if (_battleService == null || _cardService == null || _playerState == null)
                return;

            RefreshStats();
            RefreshHand();
            RefreshButtons();
            RefreshBattleResult();
        }

        private void RefreshStats()
        {
            EnemyState enemy = _battleService.CurrentEnemy;

            if (battleHudView != null)
                battleHudView.Refresh(_playerState, enemy);

            if (battleStatusView != null)
                battleStatusView.Refresh(_playerState, enemy);

            if (understandingView != null && _distortionService != null) 
                understandingView.Refresh(_distortionService.Understanding,DistortionService.MaxUnderstanding);

            if (relicBarView != null)
                relicBarView.Refresh(_playerState.Relics);
        }

        private void RefreshHand()
        {
            if (IsBusy())
                return;

            if (handView != null)
                handView.Refresh(_cardService.Hand);
        }

        private void RefreshButtons()
        {
            if (battleTurnFlow != null)
                battleTurnFlow.RefreshButton();

            if (battleResultFlow != null)
                battleResultFlow.SetContinueInteractable(_battleService.IsBattleFinished);
        }

        private void RefreshBattleResult()
        {
            if (!_battleService.IsBattleFinished)
            {
                _understandingGainShown = false;

                if (battleResultFlow != null)
                    battleResultFlow.Refresh(false, false);

                return;
            }

            if (battleResultFlow != null)
                battleResultFlow.Refresh(_battleService.IsBattleFinished, _battleService.PlayerWon);

            if (!_understandingGainShown && _battleService.PlayerWon && understandingView != null && _distortionService != null)
            {
                _understandingGainShown = true;
                understandingView.PlayGain(_understandingBeforeBattle, _distortionService.Understanding,DistortionService.MaxUnderstanding);
            }
        }

        private bool IsBusy()
        {
            return _isBusy != null && _isBusy.Invoke();
        }
    }
}