using System.Collections;
using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain.Battle;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Players;
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

        [Header("Card Result UI")]
        [SerializeField] private TMP_Text cardResultText;
        [SerializeField] private GameObject cardResultPanel;

        [Header("Battle Config")]
        [SerializeField] private List<CardSO> startingDeck;
        [SerializeField] private string enemyName = "Баба-Яга";
        [SerializeField] private int enemyHp = 40;
        [SerializeField] private int enemyDamage = 6;

        private IBattleService _battleService;
        private ICardService _cardService;
        private PlayerState _playerState;
        private IObjectResolver _resolver;

        private Coroutine _finishRoutine;
        private Coroutine _cardResultRoutine;

        [Inject]
        public void Construct(
            IBattleService battleService,
            ICardService cardService,
            PlayerState playerState,
            IObjectResolver resolver)
        {
            _battleService = battleService;
            _cardService = cardService;
            _playerState = playerState;
            _resolver = resolver;
        }

        private void Start()
        {
            if (victoryPanel != null)
                victoryPanel.SetActive(false);

            if (cardResultPanel != null)
                cardResultPanel.SetActive(false);

            if (_battleService == null || _cardService == null || _playerState == null || _resolver == null)
            {
                Debug.LogError("BattleScreen: dependencies were not injected. Check GameLifetimeScope and VContainer registration.");
                return;
            }

            if (endTurnButton != null)
                endTurnButton.onClick.AddListener(OnEndTurnClicked);

            _battleService.OnBattleStateChanged += RefreshUI;
            _battleService.OnCardPlayed += OnCardPlayed;

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

            _cardService.InitializeDeck(startingDeck);

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

            if (_battleService != null)
            {
                _battleService.OnBattleStateChanged -= RefreshUI;
                _battleService.OnCardPlayed -= OnCardPlayed;
            }
        }

        private void OnEndTurnClicked()
        {
            _battleService.EndPlayerTurn();
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
                playerHpText.text = $"HP: {_playerState.HP}/{_playerState.MaxHP}";

            if (enemyHpText != null && _battleService.CurrentEnemy != null)
                enemyHpText.text = $"HP: {_battleService.CurrentEnemy.HP}/{_battleService.CurrentEnemy.MaxHP}";

            if (playerEnergyText != null)
                playerEnergyText.text = _playerState.Energy.ToString();

            if (playerBlockText != null)
                playerBlockText.text = _playerState.Block.ToString();
        }

        private void RefreshHand()
        {
            if (handRoot == null)
                return;

            ClearHand();

            IReadOnlyList<CardSO> hand = _cardService.Hand;
            for (int i = 0; i < hand.Count; i++)
            {
                CardSO card = hand[i];
                if (card == null)
                    continue;

                var cardView = _resolver.Instantiate(cardPrefab, handRoot);
                cardView.Setup(card, false);
            }
        }

        private void RefreshButtons()
        {
            if (endTurnButton != null)
                endTurnButton.interactable = !_battleService.IsBattleFinished;
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

        private void ClearHand()
        {
            for (int i = handRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(handRoot.GetChild(i).gameObject);
            }
        }

        private string BuildEffectsText(List<CardEffect> effects)
        {
            if (effects == null || effects.Count == 0)
                return "Без эффекта";

            List<string> parts = new List<string>();

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
    }
}