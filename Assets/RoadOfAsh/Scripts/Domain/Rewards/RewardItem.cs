using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Relics;
using UnityEngine;

namespace RoadOfAsh.Scripts.Domain.Rewards
{
    public sealed class RewardItem
    {
        public RewardType Type { get; }
        public CardSO Card { get; }
        public RelicSO Relic { get; }
        public int Amount { get; }
        public Sprite Icon { get; }

        public RewardItem(RewardType type, CardSO card = null, RelicSO relic = null, int amount = 0, Sprite icon = null)
        {
            Type = type;
            Card = card;
            Relic = relic;
            Amount = amount;
            Icon = icon;
        }
    }
}