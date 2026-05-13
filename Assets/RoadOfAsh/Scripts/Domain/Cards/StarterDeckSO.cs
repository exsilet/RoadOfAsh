using System.Collections.Generic;
using UnityEngine;

namespace RoadOfAsh.Scripts.Domain.Cards
{
    [CreateAssetMenu(menuName = "Road Of Ash/Cards/Starter Deck")]
    public class StarterDeckSO : ScriptableObject
    {
        [SerializeField] private List<CardSO> cards = new();

        public IReadOnlyList<CardSO> Cards => cards;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (cards == null)
                return;

            foreach (CardSO card in cards)
            {
                if (card == null)
                    continue;

                if (card.CanAppearInRewards)
                {
                    Debug.LogWarning($"StarterDeckSO: карта '{card.CardName}' находится в стартовой колоде, но CanAppearInRewards = true. " +
                                     "Стартовая карта не должна быть reward-картой.", this);
                }
            }
        }
#endif
    }
}