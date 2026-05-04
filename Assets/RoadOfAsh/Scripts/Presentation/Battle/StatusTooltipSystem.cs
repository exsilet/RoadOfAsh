using UnityEngine;
using UnityEngine.UI;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class StatusTooltipSystem : MonoBehaviour
    {
        [SerializeField] private StatusTooltipView tooltipView;
        [SerializeField] private GameObject closeArea;
        [SerializeField] private Button closeButton;

        private void Awake()
        {
            Hide();

            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(Hide);
            }
        }

        public void Show(string title, string description, RectTransform target)
        {
            if (closeArea != null)
                closeArea.SetActive(true);

            if (tooltipView != null)
                tooltipView.Show(title, description, target);
        }

        public void Hide()
        {
            if (tooltipView != null)
                tooltipView.Hide();

            if (closeArea != null)
                closeArea.SetActive(false);
        }
    }
}