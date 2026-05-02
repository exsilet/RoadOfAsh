using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain.Cards;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_", menuName = "Deckbuilder/Card")]
public class CardSO : ScriptableObject
{
    [SerializeField] private CardRarity rarity = CardRarity.Common;
    
    public string Id;
    public string CardName;

    [TextArea] public string Description;
    [TextArea] public string FlavorText;

    public CardType Type;
    public CardRarity Rarity => rarity;
    
    public int Cost;

    public List<CardEffect> Effects = new();

    public Sprite Art;
    public Sprite ArtBackdrop;

    [Header("Corruption")]
    public bool CanBeCorrupted = true;

    [Range(0f, 1f)]
    public float CorruptionChance = 0.25f;

    public List<CardEffect> CorruptedEffects = new();
    public int CorruptedCost;
}