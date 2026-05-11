using UnityEngine;

namespace RoadOfAsh.Scripts.Domain.Events
{
    [CreateAssetMenu(fileName = "Event_", menuName = "Road Of Ash/Map/Event")]
    public class EventSO : ScriptableObject
    {
        [SerializeField] private string title;
        [TextArea] [SerializeField] private string description;
        [SerializeField] private EventChoiceData[] choices;

        public string Title => title;
        public string Description => description;
        public EventChoiceData[] Choices => choices;
    }
}