using System.Collections.Generic;
using UnityEngine;

namespace RoadOfAsh.Scripts.Domain.Relics
{
    [CreateAssetMenu(menuName = "Road Of Ash/Relics/Relic Pool")]
    public class RelicPoolSO : ScriptableObject
    {
        [SerializeField] private List<RelicSO> relics = new();

        public IReadOnlyList<RelicSO> Relics => relics;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (relics == null)
                return;

            HashSet<string> ids = new();

            foreach (RelicSO relic in relics)
            {
                if (relic == null)
                    continue;

                if (string.IsNullOrWhiteSpace(relic.Id))
                {
                    Debug.LogWarning("RelicPoolSO: есть реликвия без Id.", this);
                    continue;
                }

                if (!ids.Add(relic.Id))
                {
                    Debug.LogWarning($"RelicPoolSO: повторяется Relic Id '{relic.Id}'.", this);
                }
            }
        }
#endif
    }
}