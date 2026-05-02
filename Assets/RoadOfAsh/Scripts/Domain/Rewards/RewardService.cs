using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain.Cards;
using UnityEngine;

namespace RoadOfAsh.Scripts.Domain.Rewards
{
    public class RewardService
    {
        private const float RareChance = 0.20f;
        private const float UncommonChance = 0.45f;

        public List<CardSO> GenerateCardRewards(List<CardSO> rewardPool, int count)
        {
            List<CardSO> result = new();

            if (rewardPool == null || rewardPool.Count == 0)
                return result;

            List<CardSO> availableCards = new(rewardPool);

            for (int i = 0; i < count && availableCards.Count > 0; i++)
            {
                CardRarity rarity = RollRarity();
                CardSO card = PickCardByRarity(availableCards, rarity);

                if (card == null)
                    card = availableCards[Random.Range(0, availableCards.Count)];

                result.Add(card);
                availableCards.Remove(card);
            }

            return result;
        }

        private CardRarity RollRarity()
        {
            float roll = Random.value;

            if (roll < RareChance)
                return CardRarity.Rare;

            if (roll < RareChance + UncommonChance)
                return CardRarity.Uncommon;

            return CardRarity.Common;
        }

        private CardSO PickCardByRarity(List<CardSO> pool, CardRarity rarity)
        {
            List<CardSO> filtered = new();

            foreach (CardSO card in pool)
            {
                if (card != null && card.Rarity == rarity)
                    filtered.Add(card);
            }

            if (filtered.Count == 0)
                return null;

            return filtered[Random.Range(0, filtered.Count)];
        }
    }
}