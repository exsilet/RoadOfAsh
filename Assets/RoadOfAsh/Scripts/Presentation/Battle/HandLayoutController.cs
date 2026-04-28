using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class HandLayoutController : MonoBehaviour
    {
        [SerializeField] private RectTransform handRoot;

        [Header("Bounds")]
        [SerializeField] private float minX = -500f;
        [SerializeField] private float maxX = 500f;
        [SerializeField] private float y = 0f;

        [Header("Card")]
        [SerializeField] private float cardWidth = 220f;
        [SerializeField] private float cardHeight = 320f;

        [Header("Spacing")]
        [SerializeField] private float normalSpacing = 14f;
        [SerializeField] private float minCenterDistance = 50f;
        [SerializeField] private int compactCount = 5;

        [Header("Animation")]
        [SerializeField] private float moveDuration = 0.25f;
        [SerializeField] private float appearDuration = 0.25f;
        [SerializeField] private float appearYOffset = -80f;
        [SerializeField] private float appearScale = 0.9f;
        [SerializeField] private float stagger = 0.025f;
        
        [Header("Fan Layout")]
        [SerializeField] private bool useFanLayout = true;
        [SerializeField] private float maxRotation = 10f;
        [SerializeField] private float arcHeight = 35f;

        private readonly HashSet<RectTransform> _knownCards = new();

        public void Rebuild()
        {
            if (handRoot == null)
                return;

            int count = handRoot.childCount;
            if (count == 0)
                return;

            float normalStep = cardWidth + normalSpacing;
            float availableWidth = maxX - minX;

            float step;

            if (count == 1)
                step = 0f;
            else if (count <= compactCount)
                step = normalStep;
            else
                step = Mathf.Clamp(availableWidth / (count - 1), minCenterDistance, normalStep);

            float totalSpan = (count - 1) * step;
            float centerX = (minX + maxX) * 0.5f;
            float startX = centerX - totalSpan * 0.5f;

            for (int i = 0; i < count; i++)
            {
                RectTransform card = handRoot.GetChild(i) as RectTransform;
                if (card == null)
                    continue;

                card.anchorMin = new Vector2(0.5f, 0.5f);
                card.anchorMax = new Vector2(0.5f, 0.5f);
                card.pivot = new Vector2(0.5f, 0.5f);
                card.sizeDelta = new Vector2(cardWidth, cardHeight);
                card.SetSiblingIndex(i);

                float t = count <= 1 ? 0f : i / (float)(count - 1);
                float centeredT = t - 0.5f;

                float x = startX + i * step;
                float fanY = useFanLayout
                    ? y - Mathf.Abs(centeredT) * arcHeight
                    : y;

                float rotationZ = useFanLayout
                    ? -centeredT * maxRotation
                    : 0f;

                Vector2 targetPosition = new Vector2(x, fanY);
                Quaternion targetRotation = Quaternion.Euler(0f, 0f, rotationZ);

                bool isNewCard = !_knownCards.Contains(card);
                _knownCards.Add(card);

                card.DOKill();

                if (isNewCard)
                {
                    card.anchoredPosition = targetPosition + new Vector2(0f, appearYOffset);
                    card.localScale = Vector3.one * appearScale;

                    card.DOAnchorPos(targetPosition, appearDuration)
                        .SetEase(Ease.OutCubic)
                        .SetDelay(i * stagger);

                    card.DOScale(Vector3.one, appearDuration)
                        .SetEase(Ease.OutBack)
                        .SetDelay(i * stagger);
                    
                    card.DORotateQuaternion(targetRotation, appearDuration)
                        .SetEase(Ease.OutCubic)
                        .SetDelay(i * stagger);
                }
                else
                {
                    card.DOAnchorPos(targetPosition, moveDuration)
                        .SetEase(Ease.OutCubic);

                    card.DOScale(Vector3.one, moveDuration)
                        .SetEase(Ease.OutCubic);
                    
                    card.DORotateQuaternion(targetRotation, moveDuration)
                        .SetEase(Ease.OutCubic);
                }
            }
        }

        public void ForgetCard(RectTransform card)
        {
            if (card != null)
                _knownCards.Remove(card);
        }

        public void ClearKnownCards()
        {
            _knownCards.Clear();
        }
    }
}