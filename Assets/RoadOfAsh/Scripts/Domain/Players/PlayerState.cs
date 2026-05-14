using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Relics;

namespace RoadOfAsh.Scripts.Domain.Players
{
    public class PlayerState
    {
        public int HP { get; set; } = 30;
        public int MaxHP { get; set; } = 30;
        public int Energy { get; set; } = 3;
        public int Block { get; set; } = 0;

        public int Weak { get; set; } = 0;
        public int Poison { get; set; } = 0;

        public List<CardSO> Deck { get; } = new();
        public List<CardSO> Hand { get; } = new();
        public List<CardSO> Discard { get; } = new();
        
        public List<RelicSO> Relics = new();
        
        public void Heal(int amount)
        {
            if (amount <= 0)
                return;

            HP += amount;

            if (HP > MaxHP)
                HP = MaxHP;
        }
    }
}