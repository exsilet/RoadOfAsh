using UnityEngine;

namespace RoadOfAsh.Scripts.Presentation.Tutorial
{
    public class TutorialHighlightView : MonoBehaviour
    {
        [SerializeField] private RectTransform root;
        [SerializeField] private RectTransform canvasRoot;
        [SerializeField] private Vector2 padding = new Vector2(24f, 24f);

        private readonly Vector3[] _corners = new Vector3[4];

        public void Show(RectTransform target)
        {
            if (root == null || canvasRoot == null || target == null)
                return;

            root.gameObject.SetActive(true);

            target.GetWorldCorners(_corners);

            Vector2 min = WorldToCanvasPoint(_corners[0]);
            Vector2 max = WorldToCanvasPoint(_corners[2]);

            Vector2 size = max - min;
            Vector2 center = min + size * 0.5f;

            root.anchorMin = new Vector2(0.5f, 0.5f);
            root.anchorMax = new Vector2(0.5f, 0.5f);
            root.pivot = new Vector2(0.5f, 0.5f);

            root.anchoredPosition = center;
            root.sizeDelta = size + padding;
        }

        public void Hide()
        {
            if (root != null)
                root.gameObject.SetActive(false);
        }

        private Vector2 WorldToCanvasPoint(Vector3 worldPosition)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRoot,
                RectTransformUtility.WorldToScreenPoint(null, worldPosition),
                null,
                out Vector2 localPoint);

            return localPoint;
        }
    }
}