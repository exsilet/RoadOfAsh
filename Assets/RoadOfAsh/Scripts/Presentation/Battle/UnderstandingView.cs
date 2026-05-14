using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class UnderstandingView : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private RectTransform animatedRoot;
        [SerializeField] private Image fillImage;
        [SerializeField] private TMP_Text valueText;
        [SerializeField] private TMP_Text descriptionText;

        [Header("Text")]
        [SerializeField] private string valueFormat = "{0}/{1}";
        [SerializeField] private string description = "Чем выше понимание, тем реже случайные искажения. Некоторые враги всё ещё могут исказить карту намеренно.";

        [Header("Animation")]
        [SerializeField] private float fillDuration = 0.45f;
        [SerializeField] private float punchScale = 0.12f;
        [SerializeField] private float punchDuration = 0.25f;

        private int _currentValue;
        private int _currentMaxValue = 10;

        public void Refresh(int value, int maxValue)
        {
            if (root != null)
                root.SetActive(true);

            _currentValue = Mathf.Clamp(value, 0, maxValue);
            _currentMaxValue = Mathf.Max(1, maxValue);

            SetInstant(_currentValue, _currentMaxValue);
        }

        public void PlayGain(int oldValue, int newValue, int maxValue)
        {
            if (root != null)
                root.SetActive(true);

            int safeMax = Mathf.Max(1, maxValue);
            int from = Mathf.Clamp(oldValue, 0, safeMax);
            int to = Mathf.Clamp(newValue, 0, safeMax);

            _currentValue = to;
            _currentMaxValue = safeMax;

            if (fillImage != null)
            {
                fillImage.DOKill();

                float fromFill = (float)from / safeMax;
                float toFill = (float)to / safeMax;

                fillImage.fillAmount = fromFill;
                fillImage.DOFillAmount(toFill, fillDuration);
            }

            if (valueText != null)
                valueText.text = string.Format(valueFormat, to, safeMax);

            if (descriptionText != null)
                descriptionText.text = description;

            if (animatedRoot != null)
            {
                animatedRoot.DOKill();
                animatedRoot.localScale = Vector3.one;
                animatedRoot.DOPunchScale(Vector3.one * punchScale, punchDuration, 8, 0.7f);
            }
        }

        public void Hide()
        {
            if (root != null)
                root.SetActive(false);
        }

        private void SetInstant(int value, int maxValue)
        {
            if (fillImage != null)
                fillImage.fillAmount = maxValue > 0 ? (float)value / maxValue : 0f;

            if (valueText != null)
                valueText.text = string.Format(valueFormat, value, maxValue);

            if (descriptionText != null)
                descriptionText.text = description;
        }
    }
}