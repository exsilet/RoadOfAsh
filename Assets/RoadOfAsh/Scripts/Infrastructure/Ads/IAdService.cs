using System;

namespace RoadOfAsh.Scripts.Infrastructure.Ads
{
    public interface IAdService
    {
        bool IsRewardedAvailable { get; }

        void ShowRewarded(string rewardId, Action onSuccess, Action onFailed = null);
        void ShowInterstitial();
    }
}