using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain.Cards;

namespace RoadOfAsh.Scripts.Domain.Rewards
{
    public interface IRewardService
    {
        List<CardSO> GenerateCardRewards(List<CardSO> rewardPool, int count);
        List<CardSO> GenerateCardRewards(RewardPoolSO rewardPool, int count);

        List<RewardItem> GenerateBattleRewards(List<CardSO> rewardPool);
        List<RewardItem> GenerateBattleRewards(RewardPoolSO rewardPool);
    }
}