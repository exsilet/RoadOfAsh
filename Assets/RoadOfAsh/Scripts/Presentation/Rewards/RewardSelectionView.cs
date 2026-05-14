using System;
using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain.Rewards;
using UnityEngine;
using UnityEngine.UI;

namespace RoadOfAsh.Scripts.Presentation.Rewards
{
    public class RewardSelectionView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Transform rewardRoot;
        [SerializeField] private RewardItemView rewardItemPrefab;
        [SerializeField] private Button skipButton;
        [SerializeField] private RewardPoolSO rewardPool;

        [Header("Icons")]
        [SerializeField] private Sprite goldRewardIcon;
        [SerializeField] private Sprite healRewardIcon;

        private IRewardService _rewardService;
        private Action<RewardItem> _onRewardSelected;
        private Action _onRewardSkipped;

        private void OnDestroy()
        {
            if (skipButton != null)
                skipButton.onClick.RemoveListener(OnSkipClicked);
        }

        public void Initialize(IRewardService rewardService, Action<RewardItem> onRewardSelected, Action onRewardSkipped)
        {
            _rewardService = rewardService;
            _onRewardSelected = onRewardSelected;
            _onRewardSkipped = onRewardSkipped;

            Hide();

            if (skipButton != null)
            {
                skipButton.onClick.RemoveAllListeners();
                skipButton.onClick.AddListener(OnSkipClicked);
            }
        }

        public void Show()
        {
            if (panel != null)
                panel.SetActive(true);

            BuildRewards();
        }
        
        public void Hide()
        {
            if (panel != null)
                panel.SetActive(false);
        }
        
        public void ShowFixed(List<RewardItem> rewards)
        {
            if (panel != null)
                panel.SetActive(true);

            BuildFixedRewards(rewards);
        }
        
        private void BuildFixedRewards(List<RewardItem> rewards)
        {
            if (rewardRoot == null || rewardItemPrefab == null)
            {
                Debug.LogError("RewardSelectionView: rewardRoot or rewardItemPrefab is not assigned.");
                return;
            }

            ClearRewards();

            if (rewards == null || rewards.Count == 0)
            {
                Debug.LogError("RewardSelectionView: fixed rewards list is empty.");
                return;
            }

            rewards = AddIcons(rewards);

            foreach (RewardItem reward in rewards)
            {
                RewardItemView view = Instantiate(rewardItemPrefab, rewardRoot);
                view.Setup(reward, OnRewardClicked);
            }
        }

        private void BuildRewards()
        {
            if (rewardRoot == null || rewardItemPrefab == null)
            {
                Debug.LogError("RewardSelectionView: rewardRoot or rewardItemPrefab is not assigned.");
                return;
            }

            ClearRewards();

            if (_rewardService == null || rewardPool == null)
            {
                Debug.LogError("RewardSelectionView: reward service or reward pool is missing.");
                return;
            }

            List<RewardItem> rewards = _rewardService.GenerateBattleRewards(rewardPool);
            rewards = AddIcons(rewards);

            foreach (RewardItem reward in rewards)
            {
                RewardItemView view = Instantiate(rewardItemPrefab, rewardRoot);
                view.Setup(reward, OnRewardClicked);
            }
        }

        private List<RewardItem> AddIcons(List<RewardItem> rewards)
        {
            List<RewardItem> result = new();

            foreach (RewardItem reward in rewards)
            {
                switch (reward.Type)
                {
                    case RewardType.Gold:
                        result.Add(new RewardItem(RewardType.Gold, amount: reward.Amount, icon: goldRewardIcon));
                        break;

                    case RewardType.Heal:
                        result.Add(new RewardItem(RewardType.Heal, amount: reward.Amount, icon: healRewardIcon));
                        break;

                    default:
                        result.Add(reward);
                        break;
                }
            }

            return result;
        }

        private void ClearRewards()
        {
            for (int i = rewardRoot.childCount - 1; i >= 0; i--)
                Destroy(rewardRoot.GetChild(i).gameObject);
        }

        private void OnRewardClicked(RewardItem reward)
        {
            _onRewardSelected?.Invoke(reward);
        }

        private void OnSkipClicked()
        {
            _onRewardSkipped?.Invoke();
        }
    }
}