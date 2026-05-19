using System;
using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain.Players;

namespace RoadOfAsh.Scripts.Domain.Cards
{
    public class CardService : ICardService
    {
        private readonly PlayerState _playerState;
        private readonly List<CardSO> _deckBuffer = new();
        private readonly List<CardSO> _handBuffer = new();
        private readonly List<CardSO> _discardBuffer = new();

        public IReadOnlyList<CardSO> Hand => _handBuffer;

        public CardService(PlayerState playerState)
        {
            _playerState = playerState;
        }

        public void InitializeDeck(List<CardSO> deck, bool shuffle = true)
        {
            _deckBuffer.Clear();
            _handBuffer.Clear();
            _discardBuffer.Clear();

            _playerState.Deck.Clear();
            _playerState.Hand.Clear();
            _playerState.Discard.Clear();

            if (deck != null)
            {
                _deckBuffer.AddRange(deck);
                _playerState.Deck.AddRange(deck);
            }

            if (shuffle)
                ShuffleDeck();
        }

        public void ShuffleDeck()
        {
            var rng = new Random();

            for (int i = _deckBuffer.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (_deckBuffer[i], _deckBuffer[j]) = (_deckBuffer[j], _deckBuffer[i]);
            }
        }

        public void Draw(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (_deckBuffer.Count == 0)
                {
                    if (_discardBuffer.Count == 0)
                        return;

                    _deckBuffer.AddRange(_discardBuffer);
                    _discardBuffer.Clear();
                    _playerState.Discard.Clear();

                    ShuffleDeck();
                }

                int lastIndex = _deckBuffer.Count - 1;
                CardSO card = _deckBuffer[lastIndex];
                _deckBuffer.RemoveAt(lastIndex);

                _handBuffer.Add(card);
                _playerState.Hand.Add(card);
            }
        }

        public bool TryPlayCard(CardSO card)
        {
            if (!_handBuffer.Contains(card))
                return false;

            if (_playerState.Energy < card.Cost)
                return false;

            _playerState.Energy -= card.Cost;

            _handBuffer.Remove(card);
            _playerState.Hand.Remove(card);

            _discardBuffer.Add(card);
            _playerState.Discard.Add(card);

            return true;
        }

        public void DiscardCard(CardSO card)
        {
            if (!_handBuffer.Remove(card))
                return;

            _playerState.Hand.Remove(card);

            _discardBuffer.Add(card);
            _playerState.Discard.Add(card);
        }

        public void DiscardHand()
        {
            for (int i = _handBuffer.Count - 1; i >= 0; i--)
            {
                CardSO card = _handBuffer[i];
                _discardBuffer.Add(card);
                _playerState.Discard.Add(card);
            }

            _handBuffer.Clear();
            _playerState.Hand.Clear();
        }
    }
}