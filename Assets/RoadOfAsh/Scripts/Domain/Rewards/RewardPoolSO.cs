using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain.Cards;
using UnityEngine;

namespace RoadOfAsh.Scripts.Domain.Rewards
{
    [CreateAssetMenu(menuName = "Road Of Ash/Rewards/Reward Pool")]
    public class RewardPoolSO : ScriptableObject
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

                if (!card.CanAppearInRewards)
                {
                    Debug.LogWarning($"RewardPoolSO: карта '{card.CardName}' находится в пуле наград, но CanAppearInRewards = false. " + 
                                     "Стартовые карты не должны лежать в reward pool.",this);
                }
            }
        }
#endif
    }
}