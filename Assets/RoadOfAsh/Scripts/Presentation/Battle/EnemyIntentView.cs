using UnityEngine;
using UnityEngine.UI;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class EnemyIntentView : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Button button;

        private string _title;
        private string _description;
        private StatusTooltipSystem _tooltipSystem;

        public void Setup(Sprite icon, string title, string description, StatusTooltipSystem tooltipSystem)
        {
            _title = title;
            _description = description;
            _tooltipSystem = tooltipSystem;

            if (iconImage != null)
            {
                iconImage.sprite = icon;
                iconImage.enabled = icon != null;
            }

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(ShowTooltip);
            }
        }

        private void ShowTooltip()
        {
            if (_tooltipSystem == null)
                return;

            _tooltipSystem.Show(iconImage != null ? iconImage.sprite : null, _title, _description, transform as RectTransform);
        }
    }
}