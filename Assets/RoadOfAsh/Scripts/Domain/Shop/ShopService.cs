using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain.Cards;
using UnityEngine;

namespace RoadOfAsh.Scripts.Domain.Shop
{
    public class ShopService
    {
        public List<ShopItemData> GenerateShop(List<CardSO> pool)
        {
            List<ShopItemData> result = new();

            if (pool == null || pool.Count == 0)
                return result;

            List<CardSO> temp = new(pool);

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