using System;
using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Presentation.Battle;
using UnityEngine;
using UnityEngine.UI;

namespace RoadOfAsh.Scripts.Presentation.Map
{
    public class CardRemoveSelectionView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Transform cardRoot;
        [SerializeField] private CardView cardPrefab;
        [SerializeField] private Button closeButton;

        private Action<CardSO> _onCardSelected;

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

            BuildCards(deck);
        }

        public void Hide()
        {
            ClearCards();

            if (panel != null)
                panel.SetActive(false);
        }

        private void BuildCards(IReadOnlyList<CardSO> deck)
        {
            ClearCards();

            if (deck == null || cardRoot == null || cardPrefab == null)
                return;

            foreach (CardSO card in deck)
            {
                if (card == null)
                    continue;

                CardView cardView = Instantiate(cardPrefab, cardRoot);
                cardView.Setup(card, false);

                Button button = cardView.GetComponent<Button>();

                if (button == null)
                    button = cardView.gameObject.AddComponent<Button>();

                CardSO selectedCard = card;

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnCardClicked(selectedCard));
            }
        }

        private void OnCardClicked(CardSO card)
        {
            _onCardSelected?.Invoke(card);
        }

        private void ClearCards()
        {
            if (cardRoot == null)
                return;

            for (int i = cardRoot.childCount - 1; i >= 0; i--)
                Destroy(cardRoot.GetChild(i).gameObject);
        }
    }
}