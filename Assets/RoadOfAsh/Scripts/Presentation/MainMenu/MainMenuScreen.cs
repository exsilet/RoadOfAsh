using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Distortion;
using RoadOfAsh.Scripts.Domain.Players;
using RoadOfAsh.Scripts.Infrastructure;
using RoadOfAsh.Scripts.Infrastructure.Saves;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace RoadOfAsh.Scripts.Presentation.MainMenu
{
    public class MainMenuScreen : MonoBehaviour
    {
        [Header("Scene Names")]
        [SerializeField] private string tutorialBattleSceneName = "TutorialBattleScene";
        [SerializeField] private string mapSceneName = "MapScene";

        [Header("Buttons")]
        [SerializeField] private Button newRunButton;
        [SerializeField] private Button continueButton;

        [Header("Deck")]
        [SerializeField] private StarterDeckSO startingDeck;

        private PlayerState _playerState;
        private RunState _runState;
        private ICardService _cardService;
        private IDistortionService _distortionService;
        private ISaveService _saveService;

        [Inject]
        public void Construct(PlayerState playerState, RunState runState, ICardService cardService, IDistortionService distortionService, ISaveService saveService)
        {
            _playerState = playerState;
            _runState = runState;
            _cardService = cardService;
            _distortionService = distortionService;
            _saveService = saveService;
        }

        private void Awake()
        {
            if (newRunButton != null)
                newRunButton.onClick.AddListener(StartNewRun);

            if (continueButton != null)
                continueButton.onClick.AddListener(ContinueRun);
        }

        private void Start()
        {
            RefreshButtons();
        }

        private void OnDestroy()
        {
            if (newRunButton != null)
                newRunButton.onClick.RemoveListener(StartNewRun);

            if (continueButton != null)
                continueButton.onClick.RemoveListener(ContinueRun);
        }

        private void RefreshButtons()
        {
            if (continueButton != null)
                continueButton.interactable = _saveService != null && _saveService.HasSave;
        }

        private void StartNewRun()
        {
            _saveService?.ClearRun();

            ResetRuntimeStateForNewRunAfterTutorial();

            RunStartMode.ForceNewMap = true;

            _saveService?.SaveRun();

            RunLifetimeScope.LoadScene(mapSceneName);
        }

        private void ContinueRun()
        {
            if (_saveService != null && _saveService.TryLoadRun())
            {
                RunLifetimeScope.LoadScene(mapSceneName);
                return;
            }

            StartNewRun();
        }
        
        private void ResetRuntimeStateForNewRunAfterTutorial()
        {
            if (_runState != null)
            {
                _runState.IntroBattleCompleted = true;
                _runState.SetGold(0);
                _runState.SetSkippedRewards(0);
            }

            if (_playerState != null)
            {
                _playerState.HP = _playerState.MaxHP;
                _playerState.Block = 0;
                _playerState.Energy = 3;
                _playerState.Weak = 0;
                _playerState.Poison = 0;

                _playerState.Hand.Clear();
                _playerState.Discard.Clear();
                _playerState.Deck.Clear();
                _playerState.Relics.Clear();

                if (startingDeck == null)
                {
                    Debug.LogError("MainMenuScreen: StartingDeck is NULL. Assign StarterDeckSO in MainMenu scene.");
                    return;
                }

                foreach (CardSO card in startingDeck.Cards)
                {
                    if (card == null)
                        continue;

                    _playerState.Deck.Add(card);

                    Debug.Log($"NEW RUN START CARD: {card.CardName}, Id={card.Id}, HasUpgrade={card.HasUpgrade}");
                }

                Debug.Log($"MAIN MENU NEW RUN: HP = {_playerState.HP}/{_playerState.MaxHP}");
                Debug.Log($"MAIN MENU NEW RUN: relics count = {_playerState.Relics.Count}");
                Debug.Log($"MAIN MENU NEW RUN: deck count = {_playerState.Deck.Count}");

                bool hasUpgradeableCard = false;

                foreach (CardSO card in _playerState.Deck)
                {
                    bool cardHasUpgrade = card != null && card.HasUpgrade;

                    Debug.Log($"MAIN MENU NEW RUN CARD: {(card != null ? card.CardName : "NULL")}, HasUpgrade={cardHasUpgrade}");

                    if (cardHasUpgrade)
                        hasUpgradeableCard = true;
                }

                if (!hasUpgradeableCard)
                    Debug.LogError("MainMenuScreen: StartingDeck has no upgradeable cards. Check CardSO upgrade links.");
            }

            if (_cardService != null && startingDeck != null)
                _cardService.InitializeDeck(new List<CardSO>(startingDeck.Cards), true);

            if (_distortionService != null)
            {
                _distortionService.ResetUnderstanding();
                _distortionService.SetRandomDistortionEnabled(true);
            }
        }
    }
}