using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain.Relics;
using RoadOfAsh.Scripts.Presentation.Battle;
using UnityEngine;

namespace RoadOfAsh.Scripts.Presentation.Relics
{
    public class RelicBarView : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Transform relicRoot;
        [SerializeField] private RelicIconView relicIconPrefab;
        [SerializeField] private StatusTooltipSystem tooltipSystem;
        
        private readonly Dictionary<RelicSO, RelicIconView> _views = new();

        public void Refresh(IReadOnlyList<RelicSO> relics)
        {
            Clear();
            _views.Clear();

            bool hasRelics = relics != null && relics.Count > 0;

            if (root != null)
                root.SetActive(hasRelics);

            if (!hasRelics || relicRoot == null || relicIconPrefab == null)
                return;

            foreach (RelicSO relic in relics)
            {
                if (relic == null)
                    continue;

                RelicIconView view = Instantiate(relicIconPrefab, relicRoot);
                view.Setup(relic, tooltipSystem);
                _views[relic] = view;
            }
        }
        
        public void PlayRelicActivated(RelicSO relic)
        {
            if (relic == null)
                return;

            if (_views.TryGetValue(relic, out RelicIconView view))
                view.PlayActivate();
        }

        private void Clear()
        {
            if (relicRoot == null)
                return;

            for (int i = relicRoot.childCount - 1; i >= 0; i--)
                Destroy(relicRoot.GetChild(i).gameObject);
            
            _views.Clear();
        }
    }
}