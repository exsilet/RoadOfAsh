using System;
using RoadOfAsh.Scripts.Domain.Cards;
using UnityEngine;

namespace RoadOfAsh.Scripts.Domain.Events
{
    [Serializable]
    public class EventChoiceData
    {
        public string Title;
        [TextArea] public string Description;

        public EventChoiceType Type;
        public int Amount;
        
        [Header("Penalty")]
        public int HpCost;
        
        public CardSO Card;
    }
}