using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain.Cards;
using UnityEngine;

namespace RoadOfAsh.Scripts.Domain.Shop
{
    [CreateAssetMenu(menuName = "Road Of Ash/Shop/Shop Pool")]
    public class ShopPoolSO : ScriptableObject
    {
        [SerializeField] private List<CardSO> cards = new();

        public IReadOnlyList<CardSO> Cards => cards;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (cards == null)
                return;

            HashSet<string> ids = new();

            foreach (CardSO card in cards)
            {
                if (card == null)
                    continue;

                if (string.IsNullOrWhiteSpace(card.Id))
                {
                    Debug.LogWarning("ShopPoolSO: есть карта без Id.", this);
                    continue;
                }

                if (!ids.Add(card.Id))
                {
                    Debug.LogWarning($"ShopPoolSO: повторяется Card Id '{card.Id}'.", this);
                }

                if (!card.CanAppearInRewards)
                {
                    Debug.LogWarning(
                        $"ShopPoolSO: карта '{card.CardName}' не помечена как CanAppearInRewards. " +
                        "Если это стартовая карта — ей не место в магазине.",
                        this);
                }
            }
        }
#endif
    }
}