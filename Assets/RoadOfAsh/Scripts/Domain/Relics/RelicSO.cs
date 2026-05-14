using UnityEngine;

namespace RoadOfAsh.Scripts.Domain.Relics
{
    [CreateAssetMenu(menuName = "Road Of Ash/Relics/Relic")]
    public class RelicSO : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string relicName;
        [TextArea]
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;

        [Header("Effect")]
        [SerializeField] private RelicEffectType effectType;
        [SerializeField] private int value = 1;

        public string Id => id;
        public string RelicName => relicName;
        public string Description => description;
        public Sprite Icon => icon;
        public RelicEffectType EffectType => effectType;
        public int Value => value;
    }
}