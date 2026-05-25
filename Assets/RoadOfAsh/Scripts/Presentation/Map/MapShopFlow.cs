using System;
using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Map;
using RoadOfAsh.Scripts.Domain.Players;
using RoadOfAsh.Scripts.Domain.Relics;
using RoadOfAsh.Scripts.Domain.Shop;
using RoadOfAsh.Scripts.Infrastructure.Saves;
using UnityEngine;

namespace RoadOfAsh.Scripts.Presentation.Map
{
    public class MapShopFlow : MonoBehaviour
    {
        [SerializeField] private ShopView shopView;
        [SerializeField] private CardRemoveSelectionView cardRemoveSelectionView;
        [SerializeField] private ShopPoolSO shopPool;
        [SerializeField] private RelicPoolSO relicPool;
        [SerializeField] private int rerollCost = 15;
        [SerializeField] private int removeCardCost = 50;
        [SerializeField] private int minDeckSize = 5;

        private IMapService _mapService;
        private PlayerState _playerState;
        private RunState _runState;
        private IShopService _shopService;
        private IRelicService _relicService;
        private ISaveService _saveService;

        private List<ShopItemData> _currentItems = new();
        
        public event Action ShopCompleted;

        public void Initialize(IMapService mapService, PlayerState playerState, RunState runState, IShopService shopService, IRelicService relicService, ISaveService saveService)
        {
            _mapService = mapService;
            _playerState = playerState;
            _runState = runState;
            _shopService = shopService;
            _relicService = relicService;
            _saveService = saveService;

            if (shopView != null)
            {
                shopView.Initialize(OnBuyClicked, OnRerollClicked, OnRemoveCardClicked);
                shopView.CloseClicked += OnShopCloseClicked;
            }

            if (cardRemoveSelectionView != null)
                cardRemoveSelectionView.Hide();
        }
        
        private void OnDestroy()
        {
            if (shopView != null)
                shopView.CloseClicked -= OnShopCloseClicked;
        }

        public void Open()
        {
            _currentItems = _shopService.GenerateShop(shopPool, relicPool, _playerState.Relics);

            if (shopView != null)
                shopView.Show(_currentItems, _runState.Gold, removeCardCost);
        }
        
        private void OnShopCloseClicked()
        {
            CompleteShopNode();
        }

        private void OnBuyClicked(ShopItemData item)
        {
            if (item == null)
                return;

            if (!_runState.SpendGold(item.Price))
                return;

            switch (item.Type)
            {
                case ShopItemType.Card:
                    if (item.Card != null)
                        _playerState.Deck.Add(item.Card);
                    break;

                case ShopItemType.Relic:
                    if (item.Relic != null && _relicService != null)
                        _relicService.AddRelic(item.Relic);
                    break;
            }

            _currentItems.Remove(item);
            _saveService?.SaveRun();
            RefreshShop();
        }

        private void OnRerollClicked()
        {
            if (!_runState.SpendGold(rerollCost))
                return;

            _currentItems = _shopService.GenerateShop(shopPool, relicPool, _playerState.Relics);
            _saveService?.SaveRun();

            RefreshShop();
        }
        
        private void CompleteShopNode()
        {
            if (shopView != null)
                shopView.Hide();

            if (cardRemoveSelectionView != null)
                cardRemoveSelectionView.Hide();

            if (_mapService != null)
            {
                _mapService.CompleteSelectedNode();
                _saveService?.SaveRun();
            }

            ShopCompleted?.Invoke();
        }

        private void OnRemoveCardClicked()
        {
            if (_playerState == null || _playerState.Deck == null)
                return;

            if (_playerState.Deck.Count <= minDeckSize)
            {
                Debug.Log($"Cannot remove card: minimum deck size is {minDeckSize}.");
                return;
            }

            if (_runState == null || _runState.Gold < removeCardCost)
                return;

            if (shopView != null)
                shopView.Hide();

            if (cardRemoveSelectionView != null)
                cardRemoveSelectionView.Show(_playerState.Deck, OnCardSelectedForRemove);
        }

        private void OnCardSelectedForRemove(CardSO card)
        {
            if (card == null || _playerState == null || _playerState.Deck == null || _runState == null)
                return;

            if (_playerState.Deck.Count <= minDeckSize)
            {
                Debug.Log($"Cannot remove card: minimum deck size is {minDeckSize}.");

                if (cardRemoveSelectionView != null)
                    cardRemoveSelectionView.Hide();

                if (shopView != null)
                    shopView.Show(_currentItems, _runState.Gold, removeCardCost);

                return;
            }

            if (!_runState.SpendGold(removeCardCost))
                return;

            _playerState.Deck.Remove(card);
            _saveService?.SaveRun();

            if (cardRemoveSelectionView != null)
                cardRemoveSelectionView.Hide();

            if (shopView != null)
                shopView.Show(_currentItems, _runState.Gold, removeCardCost);
        }

        private void RefreshShop()
        {
            if (shopView != null)
                shopView.Refresh(_currentItems, _runState.Gold, removeCardCost);
        }
    }
}