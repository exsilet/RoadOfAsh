using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain.Cards;
using UnityEngine;

namespace RoadOfAsh.Scripts.Domain.Rewards
{
    public class RewardService
    {
        private const float RareChance = 0.12f;
        private const float UncommonChance = 0.38f;

        private const int MaxRewardPowerScore = 22;

        public List<CardSO> GenerateCardRewards(List<CardSO> rewardPool, int count)
        {
            List<CardSO> result = new();

            if (rewardPool == null || rewardPool.Count == 0)
                return result;

            List<CardSO> availableCards = BuildAvailableRewardPool(rewardPool);

            for (int i = 0; i < count && availableCards.Count > 0; i++)
            {
                CardRarity rarity = RollRarity();
                CardSO card = PickCardByRarity(availableCards, rarity);

                if (card == null)
                    card = PickAnyCard(availableCards);

                if (card == null)
                    break;

                result.Add(card);
                availableCards.Remove(card);
            }

            return result;
        }
        
        public List<RewardItem> GenerateBattleRewards(List<CardSO> rewardPool)
        {
            List<RewardItem> result = new();

            List<CardSO> cards = GenerateCardRewards(rewardPool, 2);

            foreach (CardSO card in cards)
            {
                result.Add(new RewardItem(RewardType.Card, card));
            }

            float roll = Random.value;

            if (roll < 0.5f)
            {
                result.Add(new RewardItem(RewardType.Gold, amount: Random.Range(20, 36)));
            }
            else
            {
                result.Add(new RewardItem(RewardType.Heal, amount: Random.Range(8, 16)));
            }

            return result;
        }

        private List<CardSO> BuildAvailableRewardPool(List<CardSO> rewardPool)
        {
            List<CardSO> result = new();

            foreach (CardSO card in rewardPool)
            {
                if (card == null)
                    continue;

                if (!card.CanAppearInRewards)
                    continue;

                if (card.PowerScore > MaxRewardPowerScore)
                    continue;

                result.Add(card);
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

        private CardSO PickAnyCard(List<CardSO> pool)
        {
            if (pool == null || pool.Count == 0)
                return null;

            return pool[Random.Range(0, pool.Count)];
        }
    }
}