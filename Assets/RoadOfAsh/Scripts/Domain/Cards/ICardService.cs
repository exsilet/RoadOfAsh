using System.Collections.Generic;

namespace RoadOfAsh.Scripts.Domain.Cards
{
    public interface ICardService
    {
        void InitializeDeck(List<CardSO> cards);
        void ShuffleDeck();
        void Draw(int count);
        bool TryPlayCard(CardSO card);
        void DiscardCard(CardSO card);
        void DiscardHand();
        IReadOnlyList<CardSO> Hand { get; }
    }
}