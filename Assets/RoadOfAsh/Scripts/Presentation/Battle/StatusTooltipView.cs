using TMPro;
using UnityEngine;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class StatusTooltipView : MonoBehaviour
    {
        [SerializeField] private RectTransform root;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Vector2 offset = new Vector2(0f, 70f);

        public void Show(string title, string description, RectTransform target)
        {
            if (root == null || target == null)
                return;

            root.gameObject.SetActive(true);

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