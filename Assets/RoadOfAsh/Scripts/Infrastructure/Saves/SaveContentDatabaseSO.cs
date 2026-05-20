using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Relics;
using UnityEngine;

namespace RoadOfAsh.Scripts.Infrastructure.Saves
{
    [CreateAssetMenu(menuName = "Road Of Ash/Saves/Save Content Database")]
    public class SaveContentDatabaseSO : ScriptableObject
    {
        [Header("Cards")]
        [SerializeField] private List<CardSO> cards = new();

        [Header("Relics")]
        [SerializeField] private List<RelicSO> relics = new();

        public CardSO GetCardById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            foreach (CardSO card in cards)
            {
                if (card != null && card.Id == id)
                    return card;
            }

            Debug.LogWarning($"SaveContentDatabaseSO: Card with id '{id}' not found.");
            return null;
        }

        public RelicSO GetRelicById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            foreach (RelicSO relic in relics)
            {
                if (relic != null && relic.Id == id)
                    return relic;
            }

            Debug.LogWarning($"SaveContentDatabaseSO: Relic with id '{id}' not found.");
            return null;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            ValidateCards();
            ValidateRelics();
        }

        private void ValidateCards()
        {
            HashSet<string> ids = new();

            foreach (CardSO card in cards)
            {
                if (card == null)
                    continue;

                if (string.IsNullOrWhiteSpace(card.Id))
                {
                    Debug.LogWarning($"SaveContentDatabaseSO: card '{card.name}' has empty Id.", this);
                    continue;
                }

                if (!ids.Add(card.Id))
                    Debug.LogWarning($"SaveContentDatabaseSO: duplicate card Id '{card.Id}'.", this);
            }
        }

        private void ValidateRelics()
        {
            HashSet<string> ids = new();

            foreach (RelicSO relic in relics)
            {
                if (relic == null)
                    continue;

                if (string.IsNullOrWhiteSpace(relic.Id))
                {
                    Debug.LogWarning($"SaveContentDatabaseSO: relic '{relic.name}' has empty Id.", this);
                    continue;
                }

                if (!ids.Add(relic.Id))
                    Debug.LogWarning($"SaveContentDatabaseSO: duplicate relic Id '{relic.Id}'.", this);
            }
        }
#endif
    }
}