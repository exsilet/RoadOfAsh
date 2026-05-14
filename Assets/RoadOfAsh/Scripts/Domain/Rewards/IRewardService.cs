using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Relics;

namespace RoadOfAsh.Scripts.Domain.Rewards
{
    public interface IRewardService
    {
        List<CardSO> GenerateCardRewards(List<CardSO> rewardPool, int count);
        List<CardSO> GenerateCardRewards(RewardPoolSO rewardPool, int count);
        
        List<RewardItem> GenerateBattleRewards(List<CardSO> rewardPool);
        List<RewardItem> GenerateBattleRewards(RewardPoolSO rewardPool);
        List<RewardItem> GenerateBattleRewards(RewardPoolSO cardPool, RelicPoolSO relicPool);
        
        RewardItem GenerateRelicReward(RelicPoolSO relicPool);
    }
}