using RoadOfAsh.Scripts.Domain.Cards;
using UnityEngine;

namespace RoadOfAsh.Scripts.Domain.Rewards
{
    public sealed class RewardItem
    {
        public RewardType Type { get; }
        public CardSO Card { get; }
        public int Amount { get; }
        public Sprite Icon { get; }

        public RewardItem(RewardType type, CardSO card = null, int amount = 0, Sprite icon = null)
        {
            Type = type;
            Card = card;
            Amount = amount;
            Icon = icon;
        }
    }
}