using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class HandView : MonoBehaviour
    {
        [SerializeField] private Transform handRoot;
        [SerializeField] private CardView cardPrefab;
        [SerializeField] private HandLayoutController handLayoutController;

        private IObjectResolver _resolver;
        private Action<CardView, CardSO> _onCardClicked;

        public Transform Root => handRoot;

        public void Initialize(IObjectResolver resolver, Action<CardView, CardSO> onCardClicked)
        {
            _resolver = resolver;
            _onCardClicked = onCardClicked;
        }

        public void Refresh(IReadOnlyList<CardSO> hand)
        {
            if (handRoot == null || cardPrefab == null || _resolver == null)
                return;

            for (int i = handRoot.childCount - 1; i >= hand.Count; i--)
            {
                Transform child = handRoot.GetChild(i);

                if (handLayoutController != null && child is RectTransform rect)
                    handLayoutController.ForgetCard(rect);

                child.SetParent(null);
                Destroy(child.gameObject);
            }

            for (int i = 0; i < hand.Count; i++)
            {
                CardSO card = hand[i];

                if (card == null)
                    continue;

                CardView cardView;

                if (i < handRoot.childCount)
                    cardView = handRoot.GetChild(i).GetComponent<CardView>();
                else
                    cardView = _resolver.Instantiate(cardPrefab, handRoot);

                cardView.Setup(card, false, _onCardClicked);
            }

            if (handLayoutController != null)
                handLayoutController.Rebuild();
        }
    }
}