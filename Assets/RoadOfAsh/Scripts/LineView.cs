using UnityEngine;

namespace RoadOfAsh.Scripts
{
    public class LineView : MonoBehaviour
    {
        public RectTransform rect;

        public void Setup(Vector2 from, Vector2 to)
        {
            Vector2 dir = to - from;
            float dist = dir.magnitude;

            rect.anchoredPosition = from;
            rect.sizeDelta = new Vector2(dist, 8f); // толщина

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rect.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}