using System;
using RoadOfAsh.Scripts.Domain.Cards;

namespace RoadOfAsh.Scripts.Domain.Shop
{
    [Serializable]
    public class ShopItemData
    {
        public CardSO Card;
        public int Price;
    }
}