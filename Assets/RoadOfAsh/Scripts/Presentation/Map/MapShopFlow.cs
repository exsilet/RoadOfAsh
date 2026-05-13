using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Map;
using RoadOfAsh.Scripts.Domain.Players;
using RoadOfAsh.Scripts.Domain.Rewards;
using RoadOfAsh.Scripts.Domain.Shop;
using UnityEngine;

namespace RoadOfAsh.Scripts.Presentation.Map
{
    public class MapShopFlow : MonoBehaviour
    {
        [SerializeField] private ShopView shopView;
        [SerializeField] private RewardPoolSO rewardPool;
        [SerializeField] private int rerollCost = 15;

        private IMapService _mapService;
        private PlayerState _playerState;
        private RunState _runState;
        private IShopService _shopService;
        private List<ShopItemData> _currentItems = new();

        public void Initialize(IMapService mapService, PlayerState playerState, RunState runState, IShopService shopService)
        {
            _mapService = mapService;
            _playerState = playerState;
            _runState = runState;
            _shopService = shopService;

            if (shopView != null)
                shopView.Initialize(OnBuyClicked, OnRerollClicked);
        }

        public void Open()
        {
            if (_shopService == null || rewardPool == null)
            {
                Debug.LogError("MapShopFlow: shop service or reward pool is missing.");
                return;
            }

            _currentItems = _shopService.GenerateShop(rewardPool);

            if (shopView != null)
                shopView.Show(_currentItems, _runState.Gold);
        }

        private void OnBuyClicked(ShopItemData item)
        {
            if (item == null || item.Card == null)
                return;

            if (!_runState.SpendGold(item.Price))
                return;

            _playerState.Deck.Add(item.Card);
            _currentItems.Remove(item);

            if (shopView != null)
                shopView.Refresh(_currentItems, _runState.Gold);
        }

        private void OnRerollClicked()
        {
            if (!_runState.SpendGold(rerollCost))
                return;

            _currentItems = _shopService.GenerateShop(rewardPool);

            if (shopView != null)
                shopView.Refresh(_currentItems, _runState.Gold);
        }
    }
}