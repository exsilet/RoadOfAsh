using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Presentation.Battle;
using UnityEngine;
using UnityEngine.UI;

namespace RoadOfAsh.Scripts.Presentation.Map
{
    public class CardUpgradeSelectionView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Transform cardRoot;
        [SerializeField] private CardView cardPrefab;
        [SerializeField] private Button closeButton;

        [Header("Upgrade Animation")] [SerializeField]
        private RectTransform hammerRect;

        [SerializeField] private Vector2 hammerStartOffset = new(0f, 220f);
        [SerializeField] private Vector2 hammerHitOffset = new(0f, 40f);
        [SerializeField] private float hammerMoveDuration = 0.18f;
        [SerializeField] private float resultHoldDuration = 0.4f;

        private Action<CardSO> _onCardSelected;
        private bool _isAnimating;

        private void Awake()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(Hide);
        }

        private void OnDestroy()
        {
            if (closeButton != null)
                closeButton.onClick.RemoveListener(Hide);
        }

        public void Show(IReadOnlyList<CardSO> deck, Action<CardSO> onCardSelected)
        {
            _onCardSelected = onCardSelected;

            if (panel != null)
                panel.SetActive(true);

            if (hammerRect != null)
                hammerRect.gameObject.SetActive(false);

            _isAnimating = false;

            BuildCards(deck);
        }

        public void Hide()
        {
            ClearCards();

            if (panel != null)
                panel.SetActive(false);

            _isAnimating = false;

            if (hammerRect != null)
                hammerRect.gameObject.SetActive(false);
        }

        private void BuildCards(IReadOnlyList<CardSO> deck)
        {
            ClearCards();

            if (deck == null || cardRoot == null || cardPrefab == null)
                return;

            foreach (CardSO card in deck)
            {
                if (card == null || !card.HasUpgrade)
                    continue;

                CardView cardView = Instantiate(cardPrefab, cardRoot);
                cardView.Setup(card, false);

                Button button = cardView.GetComponent<Button>();

                if (button != null)
                {
                    CardSO selectedCard = card;
                    CardView selectedView = cardView;

                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => OnCardClicked(selectedView, selectedCard));
                }
            }
        }

        private void OnCardClicked(CardView cardView, CardSO card)
        {
            if (_isAnimating)
                return;

            if (card == null || !card.HasUpgrade || card.UpgradedVersion == null)
                return;

            StartCoroutine(PlayUpgradeAnimation(cardView, card, card.UpgradedVersion));
        }

        private void ClearCards()
        {
            if (cardRoot == null)
                return;

            for (int i = cardRoot.childCount - 1; i >= 0; i--)
                Destroy(cardRoot.GetChild(i).gameObject);
        }

        private IEnumerator PlayUpgradeAnimation(CardView sourceCardView, CardSO sourceCard, CardSO upgradedCard)
        {
            _isAnimating = true;
            SetAllCardsInteractable(false);

            RectTransform sourceRect = sourceCardView.GetComponent<RectTransform>();

            CardView upgradedView = Instantiate(cardPrefab, sourceRect.parent);
            upgradedView.Setup(upgradedCard, false);

            RectTransform upgradedRect = upgradedView.GetComponent<RectTransform>();
            upgradedRect.anchorMin = sourceRect.anchorMin;
            upgradedRect.anchorMax = sourceRect.anchorMax;
            upgradedRect.pivot = sourceRect.pivot;
            upgradedRect.anchoredPosition = sourceRect.anchoredPosition;
            upgradedRect.sizeDelta = sourceRect.sizeDelta;
            upgradedRect.localScale = Vector3.one * 0.7f;

            CanvasGroup upgradedCanvas = upgradedView.GetComponent<CanvasGroup>();
            if (upgradedCanvas == null)
                upgradedCanvas = upgradedView.gameObject.AddComponent<CanvasGroup>();

            upgradedCanvas.alpha = 0f;

            CanvasGroup sourceCanvas = sourceCardView.GetComponent<CanvasGroup>();
            if (sourceCanvas == null)
                sourceCanvas = sourceCardView.gameObject.AddComponent<CanvasGroup>();

            if (hammerRect != null)
            {
                hammerRect.gameObject.SetActive(true);
                hammerRect.SetParent(sourceRect.parent, false);
                hammerRect.anchoredPosition = sourceRect.anchoredPosition + hammerStartOffset;
                hammerRect.localRotation = Quaternion.Euler(0f, 0f, -25f);
            }

            Sequence sequence = DOTween.Sequence();

            if (hammerRect != null)
            {
                sequence.Append(hammerRect.DOAnchorPos(sourceRect.anchoredPosition + hammerHitOffset,
                    hammerMoveDuration));
                sequence.Join(hammerRect.DORotate(Vector3.zero, hammerMoveDuration));
            }

            sequence.AppendCallback(() =>
            {
                sourceRect.DOPunchScale(new Vector3(-0.15f, -0.15f, 0f), 0.18f, 10, 0.8f);
                sourceRect.DOShakeAnchorPos(0.18f, new Vector2(18f, 10f), 12, 90f);

                sourceCanvas.DOFade(0f, 0.2f);
                upgradedCanvas.DOFade(1f, 0.22f);
                upgradedRect.DOScale(1f, 0.22f).SetEase(Ease.OutBack);
            });

            if (hammerRect != null)
            {
                sequence.AppendInterval(0.08f);
                sequence.Append(hammerRect.DOAnchorPos(sourceRect.anchoredPosition + hammerStartOffset, 0.16f));
                sequence.Join(hammerRect.DORotate(new Vector3(0f, 0f, -25f), 0.16f));
            }

            yield return sequence.WaitForCompletion();
            yield return new WaitForSeconds(resultHoldDuration);

            _onCardSelected?.Invoke(sourceCard);
        }
        
        private void SetAllCardsInteractable(bool interactable)
        {
            if (cardRoot == null)
                return;

            Button[] buttons = cardRoot.GetComponentsInChildren<Button>(true);

            foreach (Button button in buttons)
                button.interactable = interactable;
        }
    }
}