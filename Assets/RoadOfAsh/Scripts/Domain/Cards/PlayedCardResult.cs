using System.Collections.Generic;

namespace RoadOfAsh.Scripts.Domain.Cards
{
    public class PlayedCardResult
    {
        public bool WasCorrupted;
        public List<CardEffect> FinalEffects;
        public int FinalCost;
    }
}