using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain;
using RoadOfAsh.Scripts.Domain.Battle;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Distortion;
using RoadOfAsh.Scripts.Domain.Map;
using RoadOfAsh.Scripts.Domain.Players;
using UnityEngine;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class BattleSetupFlow : MonoBehaviour
    {
        [Header("Deck")]
        [SerializeField] private StarterDeckSO startingDeck;

        [Header("Enemies")]
        [SerializeField] private EnemySO tutorialEnemy;
        [SerializeField] private EnemySO fallbackEnemyConfig;

        [Header("Tutorial Cards")]
        [SerializeField] private CardSO tutorialAttackCard;
        [SerializeField] private CardSO tutorialBlockCard;
        [SerializeField] private List<CardSO> tutorialExtraCards = new();

        private IBattleService _battleService;
        private ICardService _cardService;
        private PlayerState _playerState;
        private IMapService _mapService;
        private RunState _runState;
        private IDistortionService _distortionService;

        public int UnderstandingBeforeBattle { get; private set; }
        public bool IsTutorialBattle => _runState != null && !_runState.IntroBattleCompleted;

        public void Initialize(IBattleService battleService, ICardService cardService, PlayerState playerState, IMapService mapService, RunState runState,
            IDistortionService distortionService)
        {
            _battleService = battleService;
            _cardService = cardService;
            _playerState = playerState;
            _mapService = mapService;
            _runState = runState;
            _distortionService = distortionService;
        }

        public bool TryStartBattle()
        {
            if (_battleService == null || _cardService == null || _playerState == null)
            {
                Debug.LogError("BattleSetupFlow: dependencies are missing.");
                return false;
            }

            if (startingDeck == null || startingDeck.Cards == null || startingDeck.Cards.Count == 0)
            {
                Debug.LogError("BattleSetupFlow: startingDeck is empty.");
                return false;
            }

            List<CardSO> deckForBattle = IsTutorialBattle ? BuildTutorialDeck() : BuildRegularBattleDeck();

            _playerState.Deck.Clear();
            _playerState.Deck.AddRange(deckForBattle);

            _cardService.InitializeDeck(new List<CardSO>(_playerState.Deck), !IsTutorialBattle);

            EnemySO enemy = GetEnemyForBattle();

            if (enemy == null)
            {
                Debug.LogError("BattleSetupFlow: enemy config is not assigned.");
                return false;
            }

            UnderstandingBeforeBattle = _distortionService?.Understanding ?? 0;

            _battleService.StartBattle(enemy.CreateState());
            return true;
        }

        public List<CardSO> BuildRestartDeck()
        {
            return IsTutorialBattle ? BuildTutorialDeck() : new List<CardSO>(startingDeck.Cards);
        }

        public StarterDeckSO GetStartingDeck()
        {
            return startingDeck;
        }

        private List<CardSO> BuildRegularBattleDeck()
        {
            if (_playerState.Deck.Count > 0) 
                return new List<CardSO>(_playerState.Deck);

            return new List<CardSO>(startingDeck.Cards);
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

        private EnemySO GetEnemyForBattle()
        {
            if (IsTutorialBattle)
                return tutorialEnemy;

            if (_mapService != null && _mapService.State != null)
            {
                MapNodeData selectedNode = _mapService.GetSelectedNode();

                if (selectedNode != null && selectedNode.Enemy != null)
                    return selectedNode.Enemy;
            }

            return fallbackEnemyConfig;
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
                Debug.LogWarning($"BattleSetupFlow: tutorialAttackCard '{tutorialAttackCard.CardName}' выглядит как reward-карта. " +
                                 "Для обучения лучше использовать стартовую карту атаки.",this);
            }

            if (tutorialBlockCard != null && tutorialBlockCard.CanAppearInRewards)
            {
                Debug.LogWarning($"BattleSetupFlow: tutorialBlockCard '{tutorialBlockCard.CardName}' выглядит как reward-карта. " +
                                 "Для обучения лучше использовать стартовую карту блока.",this);
            }

            if (tutorialExtraCards == null)
                return;

            foreach (CardSO card in tutorialExtraCards)
            {
                if (card == null)
                    continue;

                if (card.CanAppearInRewards)
                {
                    Debug.LogWarning($"BattleSetupFlow: tutorialExtraCards содержит reward-карту '{card.CardName}'. " +
                                     "В обучении лучше использовать только стартовые карты.",this);
                }
            }
        }
#endif
    }
}