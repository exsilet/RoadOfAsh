using RoadOfAsh.Scripts.Domain.Battle;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class CardView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image borderImage;
        [SerializeField] private Image artBackdropImage;
        [SerializeField] private Image artImage;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text typeText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text corruptedText;
        [SerializeField] private TMP_Text flavorText;
        [SerializeField] private Button button;

        [Header("Card Base Sprites")]
        [SerializeField] private Sprite attackBaseSprite;
        [SerializeField] private Sprite skillBaseSprite;
        [SerializeField] private Sprite powerBaseSprite;
        [SerializeField] private Sprite curseBaseSprite;

        private CardSO _card;
        private IBattleService _battleService;

        [Inject]
        public void Construct(IBattleService battleService)
        {
            _battleService = battleService;
        }

        public void Setup(CardSO card, bool isCorrupted = false)
        {
            _card = card;

            if (titleText != null) titleText.text = card.CardName;
            if (costText != null) costText.text = card.Cost.ToString();
            if (typeText != null) typeText.text = GetTypeLabel(card.Type);
            if (descriptionText != null) descriptionText.text = card.Description;
            if (flavorText != null) flavorText.text = card.FlavorText;

            if (borderImage != null)
            {
                borderImage.sprite = GetBaseSprite(card.Type);
                borderImage.color = Color.white;
            }

            if (artBackdropImage != null)
            {
                artBackdropImage.sprite = card.ArtBackdrop;
                artBackdropImage.enabled = card.ArtBackdrop != null;
                artBackdropImage.preserveAspect = false;
            }

            if (artImage != null)
            {
                artImage.sprite = card.Art;
                artImage.enabled = card.Art != null;
                artImage.preserveAspect = true;
            }

            if (corruptedText != null)
                corruptedText.gameObject.SetActive(isCorrupted);

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnClick);
            }
        }

        private void OnClick()
        {
            Debug.Log($"Card clicked: {_card?.CardName}, battleService={_battleService}");

            if (_card == null || _battleService == null)
                return;

            if (_battleService.IsBattleFinished)
                return;

            _battleService.TryPlayCard(_card);
        }

        private string GetTypeLabel(CardType type)
        {
            return type switch
            {
                CardType.Attack => "Атака",
                CardType.Skill => "Навык",
                CardType.Power => "Сила",
                CardType.Curse => "Проклятие",
                _ => "Карта"
            };
        }

        private Sprite GetBaseSprite(CardType type)
        {
            return type switch
            {
                CardType.Attack => attackBaseSprite,
                CardType.Skill => skillBaseSprite,
                CardType.Power => powerBaseSprite,
                CardType.Curse => curseBaseSprite,
                _ => attackBaseSprite
            };
        }
    }
}