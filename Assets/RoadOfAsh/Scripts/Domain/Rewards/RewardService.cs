using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Players;
using RoadOfAsh.Scripts.Domain.Relics;
using UnityEngine;

namespace RoadOfAsh.Scripts.Domain.Rewards
{
    public class RewardService : IRewardService
    {
        private const float RareChance = 0.12f;
        private const float UncommonChance = 0.38f;
        private const float RelicRewardChance = 0.20f;

        private const int MaxRewardPowerScore = 22;

        private readonly PlayerState _playerState;

        public RewardService(PlayerState playerState)
        {
            _playerState = playerState;
        }

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

        public List<CardSO> GenerateCardRewards(RewardPoolSO rewardPool, int count)
        {
            if (rewardPool == null)
                return new List<CardSO>();

            return GenerateCardRewards(new List<CardSO>(rewardPool.Cards), count);
        }

        public List<RewardItem> GenerateBattleRewards(List<CardSO> rewardPool)
        {
            List<RewardItem> result = new();

            List<CardSO> cards = GenerateCardRewards(rewardPool, 2);

            foreach (CardSO card in cards)
                result.Add(new RewardItem(RewardType.Card, card: card));

            result.Add(GenerateGoldOrHealReward());

            return result;
        }

        public List<RewardItem> GenerateBattleRewards(RewardPoolSO rewardPool)
        {
            if (rewardPool == null)
                return new List<RewardItem>();

            return GenerateBattleRewards(new List<CardSO>(rewardPool.Cards));
        }

        public List<RewardItem> GenerateBattleRewards(RewardPoolSO cardPool, RelicPoolSO relicPool)
        {
            List<RewardItem> result = new();

            if (cardPool == null)
                return result;

            List<CardSO> cards = GenerateCardRewards(cardPool, 2);

            foreach (CardSO card in cards)
                result.Add(new RewardItem(RewardType.Card, card: card));

            RewardItem bonusReward = TryGenerateBonusReward(relicPool);
            if (bonusReward != null)
                result.Add(bonusReward);

            return result;
        }

        public RewardItem GenerateRelicReward(RelicPoolSO relicPool)
        {
            if (relicPool == null || relicPool.Relics == null || relicPool.Relics.Count == 0)
                return null;

            List<RelicSO> available = new();

            foreach (RelicSO relic in relicPool.Relics)
            {
                if (relic == null)
                    continue;

                if (_playerState.Relics != null && _playerState.Relics.Contains(relic))
                    continue;

                available.Add(relic);
            }

            if (available.Count == 0)
                return null;

            RelicSO selected = available[Random.Range(0, available.Count)];
            return new RewardItem(RewardType.Relic, relic: selected);
        }

        private RewardItem TryGenerateBonusReward(RelicPoolSO relicPool)
        {
            if (relicPool != null && Random.value <= RelicRewardChance)
            {
                RewardItem relicReward = GenerateRelicReward(relicPool);

                if (relicReward != null)
                    return relicReward;
            }

            return GenerateGoldOrHealReward();
        }

        private RewardItem GenerateGoldOrHealReward()
        {
            float roll = Random.value;

            if (roll < 0.5f)
                return new RewardItem(RewardType.Gold, amount: Random.Range(20, 36));

            return new RewardItem(RewardType.Heal, amount: Random.Range(8, 16));
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