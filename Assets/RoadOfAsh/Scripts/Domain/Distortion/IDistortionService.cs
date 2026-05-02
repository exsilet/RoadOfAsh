using RoadOfAsh.Scripts.Domain.Cards;

namespace RoadOfAsh.Scripts.Domain.Distortion
{
    public interface IDistortionService
    {
        int Understanding { get; }
 
        void GainUnderstanding();
        void ResetUnderstanding();
        void ForceNextDistortion();
 
        bool RollDistortion(CardSO card);
        PlayedCardResult Resolve(CardSO card);
    }
}