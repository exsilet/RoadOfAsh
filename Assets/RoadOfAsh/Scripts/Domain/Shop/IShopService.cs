using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Relics;
using RoadOfAsh.Scripts.Domain.Rewards;

namespace RoadOfAsh.Scripts.Domain.Shop
{
    public interface IShopService
    {
        List<ShopItemData> GenerateShop(ShopPoolSO shopPool);
        List<ShopItemData> GenerateShop(ShopPoolSO shopPool, RelicPoolSO relicPool, IReadOnlyList<RelicSO> ownedRelics);
    }
}