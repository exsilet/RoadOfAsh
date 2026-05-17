using System;
using RoadOfAsh.Scripts.Domain.Shop;
using RoadOfAsh.Scripts.Presentation.Battle;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoadOfAsh.Scripts.Presentation.Map
{
    public class ShopItemView : MonoBehaviour
    {
        [SerializeField] private Transform cardRoot;
        [SerializeField] private CardView cardPrefab;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private Button buyButton;

        [Header("Relic Item")]
        [SerializeField] private Image relicIconImage;
        [SerializeField] private TMP_Text relicNameText;

        [Header("Tooltip")]
        [SerializeField] private Button tooltipButton;

        private ShopItemData _data;
        private StatusTooltipSystem _tooltipSystem;

        public event Action<ShopItemData> BuyClicked;

        private void Awake()
        {
            if (buyButton != null)
                buyButton.onClick.AddListener(OnBuyClicked);
        }

        private void OnDestroy()
        {
            if (buyButton != null)
                buyButton.onClick.RemoveListener(OnBuyClicked);

            if (tooltipButton != null)
                tooltipButton.onClick.RemoveListener(ShowTooltip);
        }

        public void Setup(ShopItemData data, StatusTooltipSystem tooltipSystem)
        {
            _data = data;
            _tooltipSystem = tooltipSystem;

            ClearCardRoot();
            HideRelicVisuals();

            if (tooltipButton != null)
            {
                tooltipButton.onClick.RemoveAllListeners();
                tooltipButton.onClick.AddListener(ShowTooltip);
                tooltipButton.gameObject.SetActive(data != null);
            }

            if (data == null)
            {
                if (priceText != null)
                    priceText.text = string.Empty;

                return;
            }

            switch (data.Type)
            {
                case ShopItemType.Card:
                    ShowCard(data);
                    break;

                case ShopItemType.Relic:
                    ShowRelic(data);
                    break;
            }

            if (priceText != null)
                priceText.text = data.Price.ToString();
        }

        private void ShowCard(ShopItemData data)
        {
            if (cardPrefab == null || cardRoot == null || data.Card == null)
                return;

            CardView cardView = Instantiate(cardPrefab, cardRoot);
            cardView.Setup(data.Card, false);
            DisableRaycasts(cardView.gameObject);
        }

        private void ShowRelic(ShopItemData data)
        {
            if (data.Relic == null)
                return;

            if (relicIconImage != null)
            {
                relicIconImage.gameObject.SetActive(true);
                relicIconImage.sprite = data.Relic.Icon;
                relicIconImage.enabled = data.Relic.Icon != null;
            }

            if (relicNameText != null)
            {
                relicNameText.gameObject.SetActive(true);
                relicNameText.text = data.Relic.RelicName;
            }
        }

        private void ShowTooltip()
        {
            if (_data == null || _tooltipSystem == null)
                return;

            if (_data.Type == ShopItemType.Relic && _data.Relic != null)
            {
                _tooltipSystem.Show(_data.Relic.Icon,_data.Relic.RelicName, _data.Relic.Description, transform as RectTransform);
                return;
            }

            if (_data.Type == ShopItemType.Card && _data.Card != null)
            {
                _tooltipSystem.Show(null,_data.Card.CardName, _data.Card.Description, transform as RectTransform);
            }
        }

        private void OnBuyClicked()
        {
            BuyClicked?.Invoke(_data);
        }

        private void ClearCardRoot()
        {
            if (cardRoot == null)
                return;

            for (int i = cardRoot.childCount - 1; i >= 0; i--)
                Destroy(cardRoot.GetChild(i).gameObject);
        }

        private void DisableRaycasts(GameObject root)
        {
            Graphic[] graphics = root.GetComponentsInChildren<Graphic>(true);

            foreach (Graphic graphic in graphics)
                graphic.raycastTarget = false;
        }

        private void HideRelicVisuals()
        {
            if (relicIconImage != null)
                relicIconImage.gameObject.SetActive(false);

            if (relicNameText != null)
                relicNameText.gameObject.SetActive(false);
        }
    }
}