using RoadOfAsh.Scripts.Domain.Cards;

namespace RoadOfAsh.Scripts.Domain.Distortion
{
    public interface IDistortionService
    {
        int Understanding { get; }
        bool HasForcedDistortion { get; }
 
        void GainUnderstanding();
        void ResetUnderstanding();
        void ForceNextDistortion();
        void SetRandomDistortionEnabled(bool enabled);
        void ResetTurnState();
 
        bool RollDistortion(CardSO card);
        PlayedCardResult Resolve(CardSO card);
    }
}