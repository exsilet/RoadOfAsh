using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Players;
using RoadOfAsh.Scripts.Infrastructure.Saves;
using UnityEngine;
using VContainer;

namespace RoadOfAsh.Scripts.Infrastructure
{
    public class Bootstrapper : MonoBehaviour
    {
        [Header("Scene Names")]
        [SerializeField] private string tutorialBattleSceneName = "TutorialBattleScene";
        [SerializeField] private string mapSceneName = "MapScene";
        [SerializeField] private string mainMenuSceneName = "MainMenu";

        [Header("Deck")]
        [SerializeField] private StarterDeckSO startingDeck;

        private RunState _runState;
        private PlayerState _playerState;
        private ICardService _cardService;
        private ISaveService _saveService;

        [Inject]
        public void Construct(RunState runState, PlayerState playerState, ICardService cardService, ISaveService saveService)
        {
            _runState = runState;
            _playerState = playerState;
            _cardService = cardService;
            _saveService = saveService;
        }

        private void Start()
        {
            if (_saveService != null)
                _saveService.TryLoadRun();

            // if (_runState != null && _runState.IntroBattleCompleted)
            // {
            //     EnsureDeckExistsAfterLoad();
            //     RunLifetimeScope.LoadScene(mapSceneName);
            //     return;
            // }
            //
            // RunLifetimeScope.LoadScene(tutorialBattleSceneName);
            
            if (_runState != null && !_runState.IntroBattleCompleted)
            {
                RunLifetimeScope.LoadScene(tutorialBattleSceneName);
                return;
            }
 
            RunLifetimeScope.LoadScene(mainMenuSceneName);
        }

        private void EnsureDeckExistsAfterLoad()
        {
            if (_playerState == null)
            {
                Debug.LogError("Bootstrapper: PlayerState is NULL. Cannot restore deck.");
                return;
            }

            if (_playerState.Deck.Count > 0)
                return;

            if (startingDeck == null)
            {
                Debug.LogError("Bootstrapper: StartingDeck is NULL. Assign IvanStarterDeck.");
                return;
            }

            _playerState.Hand.Clear();
            _playerState.Discard.Clear();
            _playerState.Deck.Clear();

            foreach (CardSO card in startingDeck.Cards)
            {
                if (card == null)
                    continue;

                _playerState.Deck.Add(card);
            }

            if (_cardService != null)
                _cardService.InitializeDeck(new List<CardSO>(_playerState.Deck), true);

            Debug.Log($"Bootstrapper: deck restored after load. Deck count = {_playerState.Deck.Count}");
        }
    }
}