using System;
using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain.Shop;
using RoadOfAsh.Scripts.Presentation.Battle;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoadOfAsh.Scripts.Presentation.Map
{
    public class ShopView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Transform itemsRoot;
        [SerializeField] private ShopItemView itemPrefab;
        [SerializeField] private TMP_Text goldText;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button rerollButton;
        [SerializeField] private Button removeCardButton;

        [Header("Remove Card")]
        [SerializeField] private TMP_Text removeCardPriceText;
        [SerializeField] private string removeCardPriceFormat = "Удалить карту: {0}";

        [Header("Tooltip")]
        [SerializeField] private StatusTooltipSystem tooltipSystem;

        private Action<ShopItemData> _buyCallback;
        private Action _rerollCallback;
        private Action _removeCardCallback;

        public event Action CloseClicked;

        private readonly List<ShopItemView> _spawned = new();

        public void Initialize(Action<ShopItemData> buyCallback, Action rerollCallback, Action removeCardCallback)
        {
            _buyCallback = buyCallback;
            _rerollCallback = rerollCallback;
            _removeCardCallback = removeCardCallback;

            Hide();

            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseClicked);
            }

            if (rerollButton != null)
            {
                rerollButton.onClick.RemoveAllListeners();
                rerollButton.onClick.AddListener(OnRerollClicked);
            }

            if (removeCardButton != null)
            {
                removeCardButton.onClick.RemoveAllListeners();
                removeCardButton.onClick.AddListener(OnRemoveCardClicked);
            }
        }

        private void OnDestroy()
        {
            if (closeButton != null)
                closeButton.onClick.RemoveListener(OnCloseClicked);

            if (rerollButton != null)
                rerollButton.onClick.RemoveListener(OnRerollClicked);

            if (removeCardButton != null)
                removeCardButton.onClick.RemoveListener(OnRemoveCardClicked);
        }

        public void Show(List<ShopItemData> items, int gold, int removeCardCost)
        {
            if (panel != null)
                panel.SetActive(true);

            Refresh(items, gold, removeCardCost);
        }

        public void Hide()
        {
            if (panel != null)
                panel.SetActive(false);
        }

        public void Refresh(List<ShopItemData> items, int gold, int removeCardCost)
        {
            Clear();

            if (goldText != null)
                goldText.text = gold.ToString();

            if (removeCardPriceText != null)
                removeCardPriceText.text = string.Format(removeCardPriceFormat, removeCardCost);

            if (removeCardButton != null)
                removeCardButton.interactable = gold >= removeCardCost;

            if (items == null || itemPrefab == null || itemsRoot == null)
                return;

            foreach (ShopItemData item in items)
            {
                ShopItemView view = Instantiate(itemPrefab, itemsRoot);
                view.Setup(item, tooltipSystem);
                view.BuyClicked += _buyCallback;
                _spawned.Add(view);
            }
        }

        private void Clear()
        {
            foreach (ShopItemView view in _spawned)
            {
                if (view != null)
                {
                    view.BuyClicked -= _buyCallback;
                    Destroy(view.gameObject);
                }
            }

            _spawned.Clear();
        }

        private void OnCloseClicked()
        {
            CloseClicked?.Invoke();
        }

        private void OnRerollClicked()
        {
            _rerollCallback?.Invoke();
        }

        private void OnRemoveCardClicked()
        {
            _removeCardCallback?.Invoke();
        }
    }
}