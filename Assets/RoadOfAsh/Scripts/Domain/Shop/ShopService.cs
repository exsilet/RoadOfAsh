using System.Collections.Generic;
using System.Linq;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Relics;
using UnityEngine;

namespace RoadOfAsh.Scripts.Domain.Shop
{
    public class ShopService : IShopService
    {
        private const int CardItemsCount = 3;
        private const int RelicItemsCount = 1;

        public List<ShopItemData> GenerateShop(ShopPoolSO shopPool, RelicPoolSO relicPool, IReadOnlyList<RelicSO> ownedRelics)
        {
            List<ShopItemData> result = new();

            result.AddRange(GenerateCardItems(shopPool));
            result.AddRange(GenerateRelicItems(relicPool, ownedRelics));

            return result;
        }

        public List<ShopItemData> GenerateShop(ShopPoolSO shopPool)
        {
            return GenerateShop(shopPool, null, null);
        }

        private List<ShopItemData> GenerateCardItems(ShopPoolSO shopPool)
        {
            List<ShopItemData> result = new();

            if (shopPool == null || shopPool.Cards == null || shopPool.Cards.Count == 0)
                return result;

            List<CardSO> temp = new(shopPool.Cards);

            for (int i = 0; i < CardItemsCount; i++)
            {
                if (temp.Count == 0)
                    break;

                int index = Random.Range(0, temp.Count);
                CardSO card = temp[index];
                temp.RemoveAt(index);

                if (card == null)
                    continue;

                result.Add(ShopItemData.CreateCard(card, GetCardPrice(card)));
            }

            return result;
        }

        private List<ShopItemData> GenerateRelicItems(RelicPoolSO relicPool, IReadOnlyList<RelicSO> ownedRelics)
        {
            List<ShopItemData> result = new();

            if (relicPool == null || relicPool.Relics == null || relicPool.Relics.Count == 0)
                return result;

            List<RelicSO> available = new();

            foreach (RelicSO relic in relicPool.Relics)
            {
                if (relic == null)
                    continue;

                if (ownedRelics != null && ownedRelics.Contains(relic))
                    continue;

                available.Add(relic);
            }

            for (int i = 0; i < RelicItemsCount; i++)
            {
                if (available.Count == 0)
                    break;

                int index = Random.Range(0, available.Count);
                RelicSO relic = available[index];
                available.RemoveAt(index);

                result.Add(ShopItemData.CreateRelic(relic, GetRelicPrice(relic)));
            }

            return result;
        }

        private int GetCardPrice(CardSO card)
        {
            if (card == null)
                return 50;

            return card.Rarity switch
            {
                CardRarity.Common => 40,
                CardRarity.Uncommon => 70,
                CardRarity.Rare => 120,
                _ => 50
            };
        }

        private int GetRelicPrice(RelicSO relic)
        {
            if (relic == null)
                return 100;

            return relic.EffectType switch
            {
                RelicEffectType.BlockFirstDistortionEachTurn => 120,
                RelicEffectType.GainBlockAtTurnStart => 90,
                RelicEffectType.GainGoldAfterBattle => 100,
                RelicEffectType.HealAfterBattle => 100,
                _ => 100
            };
        }
    }
}