using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class BattleEffectPopup : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text valueText;
        [SerializeField] private CanvasGroup canvasGroup;

        public void Setup(Sprite icon, int? value)
        {
            if (iconImage != null)
            {
                iconImage.sprite = icon;
                iconImage.enabled = icon != null;
            }

            if (valueText != null)
            {
                bool hasValue = value.HasValue;
                valueText.gameObject.SetActive(hasValue);

                if (hasValue)
                    valueText.text = value.Value.ToString();
            }

            if (canvasGroup != null)
                canvasGroup.alpha = 1f;
        }

        public void Play(float moveY, float duration)
        {
            RectTransform rect = GetComponent<RectTransform>();
            if (rect == null)
            {
                Destroy(gameObject, duration);
                return;
            }

            rect.anchoredPosition = Vector2.zero;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(rect.DOAnchorPosY(moveY, duration));

            if (canvasGroup != null)
                sequence.Join(canvasGroup.DOFade(0f, duration));

            sequence.OnComplete(() => Destroy(gameObject));
        }
    }
}