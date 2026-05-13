using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Rewards;
using UnityEngine;

namespace RoadOfAsh.Scripts.Domain.Shop
{
    public class ShopService : IShopService
    {
        public List<ShopItemData> GenerateShop(RewardPoolSO rewardPool)
        {
            if (rewardPool == null)
                return new List<ShopItemData>();

            return GenerateShop(new List<CardSO>(rewardPool.Cards));
        }

        public List<ShopItemData> GenerateShop(List<CardSO> pool)
        {
            List<ShopItemData> result = new();

            if (pool == null || pool.Count == 0)
                return result;

            List<CardSO> temp = BuildAvailableShopPool(pool);

            for (int i = 0; i < 3; i++)
            {
                if (temp.Count == 0)
                    break;

                int index = Random.Range(0, temp.Count);
                CardSO card = temp[index];

                temp.RemoveAt(index);

                result.Add(new ShopItemData
                {
                    Card = card,
                    Price = GetPrice(card)
                });
            }

            return result;
        }

        private List<CardSO> BuildAvailableShopPool(List<CardSO> pool)
        {
            List<CardSO> result = new();

            foreach (CardSO card in pool)
            {
                if (card == null)
                    continue;

                if (!card.CanAppearInRewards)
                    continue;

                result.Add(card);
            }

            return result;
        }

        private int GetPrice(CardSO card)
        {
            return card.Rarity switch
            {
                CardRarity.Common => 40,
                CardRarity.Uncommon => 70,
                CardRarity.Rare => 120,
                _ => 50
            };
        }
    }
}