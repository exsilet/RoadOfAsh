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

        private ShopItemData _data;

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
        }

        public void Setup(ShopItemData data)
        {
            _data = data;

            ClearCardRoot();

            if (data == null || data.Card == null)
                return;

            if (cardPrefab != null && cardRoot != null)
            {
                CardView cardView = Instantiate(cardPrefab, cardRoot);
                cardView.Setup(data.Card, false);
                DisableRaycasts(cardView.gameObject);
            }

            if (priceText != null)
                priceText.text = data.Price.ToString();
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
    }
}