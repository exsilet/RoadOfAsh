using System;
using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Players;
using RoadOfAsh.Scripts.Domain.Relics;
using RoadOfAsh.Scripts.Domain.Rewards;
using RoadOfAsh.Scripts.Presentation.Rewards;
using UnityEngine;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class BattleRewardFlow : MonoBehaviour
    {
        [SerializeField] private RewardSelectionView rewardSelectionView;

        [Header("Tutorial Reward")]
        [SerializeField] private bool showTutorialReward = true;
        [SerializeField] private CardSO tutorialRewardCard;

        private PlayerState _playerState;
        private RunState _runState;
        private IRelicService _relicService;
        private IRewardService _rewardService;

        private bool _rewardShown;

        public event Action Completed;

        public void Initialize(
            PlayerState playerState,
            RunState runState,
            IRelicService relicService,
            IRewardService rewardService)
        {
            _playerState = playerState;
            _runState = runState;
            _relicService = relicService;
            _rewardService = rewardService;

            _rewardShown = false;

            if (rewardSelectionView != null)
            {
                rewardSelectionView.Initialize(
                    _rewardService,
                    OnRewardSelected,
                    OnSkipRewardClicked);
            }
        }

        public bool CanShowTutorialReward()
        {
            return showTutorialReward && tutorialRewardCard != null;
        }

        public void ShowRegularReward()
        {
            if (_rewardShown)
                return;

            _rewardShown = true;

            if (rewardSelectionView == null)
            {
                Complete();
                return;
            }

            rewardSelectionView.Show();
        }

        public void ShowTutorialReward()
        {
            if (_rewardShown)
                return;

            _rewardShown = true;

            if (rewardSelectionView == null || tutorialRewardCard == null)
            {
                Complete();
                return;
            }

            List<RewardItem> rewards = new()
            {
                new RewardItem(RewardType.Card, card: tutorialRewardCard)
            };

            rewardSelectionView.ShowFixed(rewards);
        }

        private void OnRewardSelected(RewardItem reward)
        {
            if (reward == null)
                return;

            ApplyReward(reward);
            Complete();
        }

        private void OnSkipRewardClicked()
        {
            if (_runState != null)
                _runState.AddSkippedReward();

            Complete();
        }

        private void ApplyReward(RewardItem reward)
        {
            switch (reward.Type)
            {
                case RewardType.Card:
                    if (reward.Card != null && _playerState != null)
                        _playerState.Deck.Add(reward.Card);
                    break;
                case RewardType.Gold:
                    if (_runState != null)
                        _runState.AddGold(reward.Amount);
                    break;
                case RewardType.Heal:
                    if (_playerState != null)
                        _playerState.Heal(reward.Amount);
                    break;
                case RewardType.Relic:
                    if (reward.Relic != null && _relicService != null)
                        _relicService.AddRelic(reward.Relic);
                    break;
            }
        }

        private void Complete()
        {
            if (rewardSelectionView != null)
                rewardSelectionView.Hide();

            Completed?.Invoke();
        }
    }
}