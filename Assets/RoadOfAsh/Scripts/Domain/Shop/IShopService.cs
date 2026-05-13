using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Rewards;

namespace RoadOfAsh.Scripts.Domain.Shop
{
    public interface IShopService
    {
        List<ShopItemData> GenerateShop(List<CardSO> pool);
        List<ShopItemData> GenerateShop(RewardPoolSO rewardPool);
    }
}