using RoadOfAsh.Scripts.Domain.Cards;
using UnityEngine;
using VContainer;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class BattleHandFlow : MonoBehaviour
    {
        [SerializeField] private HandView handView;
        [SerializeField] private BattleCardPlayFlow battleCardPlayFlow;

        private IObjectResolver _resolver;

        public void Initialize(IObjectResolver resolver)
        {
            _resolver = resolver;

            if (handView != null)
                handView.Initialize(_resolver, OnCardViewClicked);
        }

        private void OnCardViewClicked(CardView cardView, CardSO card)
        {
            if (battleCardPlayFlow != null)
                battleCardPlayFlow.TryHandleCardClicked(cardView, card);
        }
    }
}