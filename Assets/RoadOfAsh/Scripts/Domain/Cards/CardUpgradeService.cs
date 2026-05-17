using RoadOfAsh.Scripts.Domain.Players;

namespace RoadOfAsh.Scripts.Domain.Cards
{
    public class CardUpgradeService
    {
        private readonly PlayerState _playerState;

        public CardUpgradeService(PlayerState playerState)
        {
            _playerState = playerState;
        }

        public bool CanUpgradeAnyCard()
        {
            if (_playerState?.Deck == null)
                return false;

            foreach (CardSO card in _playerState.Deck)
            {
                if (card != null && card.HasUpgrade)
                    return true;
            }

            return false;
        }

        public bool TryUpgradeCard(CardSO card)
        {
            if (card == null || !card.HasUpgrade)
                return false;

            if (_playerState?.Deck == null)
                return false;

            int index = _playerState.Deck.IndexOf(card);

            if (index < 0)
                return false;

            _playerState.Deck[index] = card.UpgradedVersion;
            return true;
        }
    }
}