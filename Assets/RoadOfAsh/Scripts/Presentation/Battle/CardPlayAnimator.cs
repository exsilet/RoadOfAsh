using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class CardPlayAnimator : MonoBehaviour
    {
        [SerializeField] private RectTransform playTarget;
        [SerializeField] private RectTransform discardTarget;
        [SerializeField] private float playMoveDuration = 0.22f;
        [SerializeField] private float discardMoveDuration = 0.28f;
        [SerializeField] private float discardScale = 0.25f;
        [SerializeField] private float discardStagger = 0.05f;

        public IEnumerator MoveToPlay(CardView cardView)
        {
            if (cardView == null || playTarget == null)
                yield break;

            RectTransform cardRect = cardView.GetComponent<RectTransform>();

            if (cardRect == null)
                yield break;

            cardRect.DOKill();
            cardRect.SetAsLastSibling();

            yield return cardRect
                .DOMove(playTarget.position, playMoveDuration)
                .SetEase(Ease.OutCubic)
                .WaitForCompletion();
        }
        
        public IEnumerator MoveToDiscard(CardView cardView)
        {
            if (cardView == null || discardTarget == null)
                yield break;

            RectTransform cardRect = cardView.GetComponent<RectTransform>();

            if (cardRect == null)
                yield break;

            cardRect.DOKill();

            Tween moveTween = cardRect
                .DOMove(discardTarget.position, discardMoveDuration)
                .SetEase(Ease.InCubic);

            cardRect
                .DOScale(Vector3.one * discardScale, discardMoveDuration)
                .SetEase(Ease.InCubic);

            yield return moveTween.WaitForCompletion();
        }

        public IEnumerator DiscardHand(Transform handRoot)
        {
            if (handRoot == null || discardTarget == null)
                yield break;

            int count = handRoot.childCount;

            for (int i = count - 1; i >= 0; i--)
            {
                RectTransform cardRect = handRoot.GetChild(i) as RectTransform;

                if (cardRect == null)
                    continue;

                cardRect.DOKill();
                cardRect.SetAsLastSibling();

                float delay = (count - 1 - i) * discardStagger;

                cardRect.DOMove(discardTarget.position, discardMoveDuration)
                    .SetEase(Ease.InCubic)
                    .SetDelay(delay);

                cardRect.DOScale(Vector3.one * discardScale, discardMoveDuration)
                    .SetEase(Ease.InCubic)
                    .SetDelay(delay);
            }

            yield return new WaitForSeconds(discardMoveDuration + count * discardStagger);
        }

        public void Shake(CardView cardView)
        {
            if (cardView == null)
                return;

            RectTransform rect = cardView.GetComponent<RectTransform>();

            if (rect == null)
                return;

            rect.DOKill();
            rect.DOShakeAnchorPos(0.25f, new Vector2(18f, 0f), 12, 90f);
        }
    }
}