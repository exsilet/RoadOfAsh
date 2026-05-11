using RoadOfAsh.Scripts.Domain.Cards;
using UnityEngine;

namespace RoadOfAsh.Scripts.Domain.Distortion
{
    public class DistortionService : IDistortionService
    {
        public const int MaxUnderstanding = 10;
        private const float UnderstandingReductionPerLevel = 0.09f;
 
        public int Understanding { get; private set; }
 
        private bool _forceNext;
        
        public bool HasForcedDistortion => _forceNext;
 
        public void GainUnderstanding()
        {
            if (Understanding < MaxUnderstanding)
                Understanding++;
        }
 
        public void ResetUnderstanding()
        {
            Understanding = 0;
            _forceNext = false;
        }
 
        public void ForceNextDistortion()
        {
            _forceNext = true;
        }
 
        public bool RollDistortion(CardSO card)
        {
            if (_forceNext)
                return true;
 
            if (card == null || !card.CanBeCorrupted)
                return false;
 
            if (Understanding >= MaxUnderstanding)
                return false;
 
            float reductionFactor = 1f - Understanding * UnderstandingReductionPerLevel;
            float effectiveChance = card.CorruptionChance * Mathf.Clamp01(reductionFactor);
 
            return Random.value <= effectiveChance;
        }
 
        public PlayedCardResult Resolve(CardSO card)
        {
            bool corrupted = (card.CanBeCorrupted || _forceNext)
                             && card.CorruptedEffects.Count > 0
                             && RollDistortion(card);
 
            _forceNext = false;
 
            if (corrupted)
            {
                return new PlayedCardResult
                {
                    WasCorrupted = true,
                    FinalEffects = card.CorruptedEffects,
                    FinalCost = card.CorruptedCost > 0 ? card.CorruptedCost : card.Cost
                };
            }
 
            return new PlayedCardResult
            {
                WasCorrupted = false,
                FinalEffects = card.Effects,
                FinalCost = card.Cost
            };
        }
    }
}