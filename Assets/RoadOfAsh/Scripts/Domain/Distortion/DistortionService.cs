using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Relics;
using UnityEngine;

namespace RoadOfAsh.Scripts.Domain.Distortion
{
    public class DistortionService : IDistortionService
    {
        public const int MaxUnderstanding = 10;

        private const float MaxRandomDistortionReduction = 0.80f;
        private const float MinRandomDistortionChance = 0.05f;
        
        private bool _randomDistortionEnabled = true;
        private bool _forceNext;
 
        public int Understanding { get; private set; }
        
        public bool HasForcedDistortion => _forceNext;
        
        private readonly IRelicService _relicService;
        
        public DistortionService(IRelicService relicService)
        {
            _relicService = relicService;
        }
 
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

            if (!_randomDistortionEnabled)
                return false;

            if (card == null || !card.CanBeCorrupted)
                return false;

            float effectiveChance = CalculateRandomDistortionChance(card);

            return Random.value <= effectiveChance;
        }
        
        private float CalculateRandomDistortionChance(CardSO card)
        {
            float baseChance = Mathf.Clamp01(card.CorruptionChance);

            if (baseChance <= 0f)
                return 0f;

            int understanding = Mathf.Clamp(Understanding, 0, MaxUnderstanding);

            float progress = MaxUnderstanding > 0
                ? (float)understanding / MaxUnderstanding
                : 0f;

            float reduction = progress * MaxRandomDistortionReduction;
            float effectiveChance = baseChance * (1f - reduction);

            return Mathf.Max(MinRandomDistortionChance, effectiveChance);
        }
        
        public void SetRandomDistortionEnabled(bool enabled)
        {
            _randomDistortionEnabled = enabled;
        }
 
        public PlayedCardResult Resolve(CardSO card)
        {
            bool shouldDistort = card != null &&
                                 (card.CanBeCorrupted || _forceNext) &&
                                 card.CorruptedEffects.Count > 0 &&
                                 RollDistortion(card);

            bool wasBlockedByRelic = false;

            if (shouldDistort && _relicService != null && _relicService.TryBlockDistortion())
            {
                shouldDistort = false;
                wasBlockedByRelic = true;
            }

            _forceNext = false;

            if (shouldDistort)
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
        
        public void ResetTurnState()
        {
            _relicService?.ResetBattleRelicState();
        }
    }
}