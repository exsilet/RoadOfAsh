using System;
using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain.Rewards;
using RoadOfAsh.Scripts.Presentation.Rewards;
using UnityEngine;
using UnityEngine.UI;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class BattleRewardView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Transform rewardCardsRoot;
        [SerializeField] private RewardItemView rewardItemPrefab;
        [SerializeField] private Button skipRewardButton;
        [SerializeField] private List<CardSO> rewardPool;

        [Header("Icons")]
        [SerializeField] private Sprite goldRewardIcon;
        [SerializeField] private Sprite healRewardIcon;

        private RewardService _rewardService;
        private Action<RewardItem> _onRewardSelected;
        private Action _onRewardSkipped;

        private void OnDestroy()
        {
            if (skipRewardButton != null)
                skipRewardButton.onClick.RemoveListener(OnSkipClicked);
        }

        public void Initialize(
            RewardService rewardService,
            Action<RewardItem> onRewardSelected,
            Action onRewardSkipped)
        {
            _rewardService = rewardService;
            _onRewardSelected = onRewardSelected;
            _onRewardSkipped = onRewardSkipped;

            if (skipRewardButton != null)
            {
                skipRewardButton.onClick.RemoveAllListeners();
                skipRewardButton.onClick.AddListener(OnSkipClicked);
            }

            Hide();
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

        private void BuildRewards()
        {
            if (rewardCardsRoot == null || rewardItemPrefab == null)
            {
                Debug.LogError("BattleRewardView: rewardCardsRoot or rewardItemPrefab is not assigned.");
                return;
            }

            ClearRewards();

            if (rewardPool == null || rewardPool.Count == 0)
            {
                Debug.LogError("BattleRewardView: rewardPool is empty.");
                return;
            }

            List<RewardItem> rewards = _rewardService != null
                ? _rewardService.GenerateBattleRewards(rewardPool)
                : new List<RewardItem>();

            rewards = AddRewardIcons(rewards);

            if (rewards.Count == 0)
            {
                Debug.LogError("BattleRewardView: reward items were not generated.");
                return;
            }

            foreach (RewardItem reward in rewards)
            {
                RewardItemView view = Instantiate(rewardItemPrefab, rewardCardsRoot);
                view.Setup(reward, OnRewardClicked);
            }
        }

        private List<RewardItem> AddRewardIcons(List<RewardItem> rewards)
        {
            List<RewardItem> result = new();

            foreach (RewardItem reward in rewards)
            {
                switch (reward.Type)
                {
                    case RewardType.Gold:
                        result.Add(new RewardItem(
                            RewardType.Gold,
                            amount: reward.Amount,
                            icon: goldRewardIcon));
                        break;

                    case RewardType.Heal:
                        result.Add(new RewardItem(
                            RewardType.Heal,
                            amount: reward.Amount,
                            icon: healRewardIcon));
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
            for (int i = rewardCardsRoot.childCount - 1; i >= 0; i--)
                Destroy(rewardCardsRoot.GetChild(i).gameObject);
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