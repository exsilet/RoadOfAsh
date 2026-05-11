using System;
using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain.Shop;
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

        private Action<ShopItemData> _buyCallback;
        private Action _rerollCallback;

        private readonly List<ShopItemView> _spawned = new();

        public void Initialize(
            Action<ShopItemData> buyCallback,
            Action rerollCallback)
        {
            _buyCallback = buyCallback;
            _rerollCallback = rerollCallback;

            Hide();

            if (closeButton != null)
                closeButton.onClick.AddListener(Hide);

            if (rerollButton != null)
                rerollButton.onClick.AddListener(OnRerollClicked);
        }

        public void Show(List<ShopItemData> items, int gold)
        {
            if (panel != null)
                panel.SetActive(true);

            Refresh(items, gold);
        }

        public void Hide()
        {
            if (panel != null)
                panel.SetActive(false);
        }

        public void Refresh(List<ShopItemData> items, int gold)
        {
            Clear();

            if (goldText != null)
                goldText.text = gold.ToString();

            foreach (ShopItemData item in items)
            {
                ShopItemView view = Instantiate(itemPrefab, itemsRoot);

                view.Setup(item);
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

        private void OnRerollClicked()
        {
            _rerollCallback?.Invoke();
        }
    }
}