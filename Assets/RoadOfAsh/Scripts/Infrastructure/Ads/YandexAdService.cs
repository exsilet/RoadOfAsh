using System;
using UnityEngine;
using YG;

namespace RoadOfAsh.Scripts.Infrastructure.Ads
{
    public class YandexAdService : IAdService
    {
        public bool IsRewardedAvailable => !YG2.nowAdsShow;

        public void ShowRewarded(string rewardId, Action onSuccess, Action onFailed = null)
        {
            if (string.IsNullOrWhiteSpace(rewardId))
            {
                Debug.LogError("YandexAdService: rewardId is empty.");
                onFailed?.Invoke();
                return;
            }

            if (YG2.nowAdsShow)
            {
                Debug.LogWarning("YandexAdService: ad is already showing.");
                onFailed?.Invoke();
                return;
            }

            YG2.RewardedAdvShow(rewardId, () =>
            {
                Debug.Log($"YandexAdService: rewarded ad completed. RewardId: {rewardId}");
                onSuccess?.Invoke();
            });
        }

        public void ShowInterstitial()
        {
            if (YG2.nowAdsShow)
                return;

            YG2.InterstitialAdvShow();
        }
    }
}