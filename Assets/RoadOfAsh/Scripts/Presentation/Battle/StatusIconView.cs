using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class StatusIconView : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text valueText;
        [SerializeField] private Button button;

        private string _title;
        private string _description;
        private StatusTooltipSystem _tooltipSystem;

        public void Setup(Sprite icon, int value, string title, string description, StatusTooltipSystem tooltipSystem)
        {
            _tooltipSystem = tooltipSystem;
            _title = title;
            _description = description;

            if (iconImage != null)
                iconImage.sprite = icon;

            if (valueText != null)
                valueText.text = value.ToString();

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

            _tooltipSystem.Show(_title, _description, transform as RectTransform);
        }
    }
}