using System.Collections.Generic;
using UnityEngine;

namespace RoadOfAsh.Scripts.Domain.Cards
{
    [CreateAssetMenu(fileName = "Card_", menuName = "Deckbuilder/Card")]
    public class CardSO : ScriptableObject
    {
        public string Id;
        public string CardName;
    
        [TextArea] public string Description;
        [TextArea] public string FlavorText;

        public CardType Type;
        public int Cost;
        [SerializeField] private CardRarity rarity = CardRarity.Common;
    
        [Header("Balance")]
        [SerializeField] private int powerScore = 10;
        [SerializeField] private bool canAppearInRewards = true;

        public List<CardEffect> Effects = new();

        public Sprite Art;
        public Sprite ArtBackdrop;

        [Header("Corruption")]
        public bool CanBeCorrupted = true;

        [Range(0f, 1f)]
        public float CorruptionChance = 0.25f;
        public int CorruptedCost;

        public List<CardEffect> CorruptedEffects = new();
    
        public CardRarity Rarity => rarity;
        public int PowerScore => powerScore;
        public bool CanAppearInRewards => canAppearInRewards;
    }
}