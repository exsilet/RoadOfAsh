using DG.Tweening;
using RoadOfAsh.Scripts.Domain.Relics;
using RoadOfAsh.Scripts.Presentation.Battle;
using UnityEngine;
using UnityEngine.UI;

namespace RoadOfAsh.Scripts.Presentation.Relics
{
    public class RelicIconView : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Button button;

        private RelicSO _relic;
        private StatusTooltipSystem _tooltipSystem;

        public void Setup(RelicSO relic, StatusTooltipSystem tooltipSystem)
        {
            _relic = relic;
            _tooltipSystem = tooltipSystem;

            if (iconImage != null)
            {
                iconImage.sprite = relic != null ? relic.Icon : null;
                iconImage.enabled = relic != null && relic.Icon != null;
            }

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(ShowTooltip);
            }
        }
        
        public void PlayActivate()
        {
            RectTransform rect = transform as RectTransform;

            if (rect == null)
                return;

            rect.DOKill();
            rect.localScale = Vector3.one;
            rect.DOPunchScale(Vector3.one * 0.18f, 0.28f, 8, 0.8f);
        }

        private void ShowTooltip()
        {
            if (_relic == null || _tooltipSystem == null)
                return;

            _tooltipSystem.Show(
                _relic.Icon,
                _relic.RelicName,
                _relic.Description,
                transform as RectTransform
            );
        }

        private void OnDestroy()
        {
            if (button != null)
                button.onClick.RemoveListener(ShowTooltip);
        }
    }
}