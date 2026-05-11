using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class StatusTooltipView : MonoBehaviour
    {
        [SerializeField] private RectTransform root;
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Vector2 offset = new Vector2(0f, 70f);

        public void Show(string title, string description, RectTransform target)
        {
            Show(null, title, description, target);
        }

        public void Show(Sprite icon, string title, string description, RectTransform target)
        {
            if (root == null || target == null)
                return;

            root.gameObject.SetActive(true);

            if (iconImage != null)
            {
                bool hasIcon = icon != null;
                iconImage.gameObject.SetActive(hasIcon);

                if (hasIcon)
                    iconImage.sprite = icon;
            }

            if (titleText != null)
                titleText.text = title;

            if (descriptionText != null)
                descriptionText.text = description;

            root.position = target.position + (Vector3)offset;
        }

        public void Hide()
        {
            if (root != null)
                root.gameObject.SetActive(false);
        }
    }
}