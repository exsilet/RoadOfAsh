using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Distortion;
using RoadOfAsh.Scripts.Domain.Map;
using RoadOfAsh.Scripts.Domain.Players;
using RoadOfAsh.Scripts.Infrastructure;
using RoadOfAsh.Scripts.Infrastructure.Saves;
using UnityEngine;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class BattleSceneFlow : MonoBehaviour
    {
        [Header("Scene Names")]
        [SerializeField] private string mainMenuSceneName = "MainMenu";
        [SerializeField] private string mapSceneName = "MapScene";
        [SerializeField] private string battleSceneName = "BattleScene";

        private PlayerState _playerState;
        private ICardService _cardService;
        private IMapService _mapService;
        private RunState _runState;
        private IDistortionService _distortionService;
        private ISaveService _saveService;

        public void Initialize(PlayerState playerState, ICardService cardService, IMapService mapService, RunState runState, IDistortionService distortionService,
            ISaveService saveService)
        {
            _playerState = playerState;
            _cardService = cardService;
            _mapService = mapService;
            _runState = runState;
            _distortionService = distortionService;
            _saveService = saveService;
        }

        public void CompleteTutorialAndGoToMap()
        {
            if (_distortionService != null)
            {
                _distortionService.SetRandomDistortionEnabled(true);
                _distortionService.ResetUnderstanding();
            }

            if (_runState != null)
                _runState.IntroBattleCompleted = true;

            _saveService?.SaveRun();

            RunLifetimeScope.LoadScene(mapSceneName);
        }

        public void GoToMapAfterReward()
        {
            if (_mapService != null && _mapService.State != null && _mapService.State.SelectedNodeId >= 0)
            {
                _mapService.CompleteSelectedNode();
                _saveService?.SaveRun();
            }
            else
            {
                if (_distortionService != null)
                {
                    _distortionService.SetRandomDistortionEnabled(true);
                    _distortionService.ResetUnderstanding();
                }

                if (_runState != null)
                    _runState.IntroBattleCompleted = true;
            }

            _saveService?.SaveRun();

            RunLifetimeScope.LoadScene(mapSceneName);
        }

        public void RestartBattleAfterDefeat(StarterDeckSO startingDeck, List<CardSO> tutorialDeck, bool isTutorialBattle)
        {
            if (_playerState == null || _cardService == null)
                return;

            _playerState.HP = Mathf.Max(1, Mathf.RoundToInt(_playerState.MaxHP * 0.5f));
            _playerState.Block = 0;
            _playerState.Energy = 3;
            _playerState.Weak = 0;
            _playerState.Poison = 0;

            _playerState.Hand.Clear();
            _playerState.Discard.Clear();

            List<CardSO> deckForBattle = isTutorialBattle ? tutorialDeck : new List<CardSO>(_playerState.Deck);

            _playerState.Deck.Clear();
            _playerState.Deck.AddRange(deckForBattle);

            _cardService.InitializeDeck(new List<CardSO>(_playerState.Deck), !isTutorialBattle);

            RunLifetimeScope.LoadScene(battleSceneName);
        }

        public void GoToMainMenu()
        {
            _saveService?.SaveRun();

            RunLifetimeScope.LoadScene(mainMenuSceneName);
        }
    }
}