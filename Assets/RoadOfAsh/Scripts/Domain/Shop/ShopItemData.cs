using System;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Relics;

namespace RoadOfAsh.Scripts.Domain.Shop
{
    [Serializable]
    public class ShopItemData
    {
        public ShopItemType Type;
        public CardSO Card;
        public RelicSO Relic;
        public int Price;

        public static ShopItemData CreateCard(CardSO card, int price)
        {
            return new ShopItemData
            {
                Type = ShopItemType.Card,
                Card = card,
                Price = price
            };
        }

        public static ShopItemData CreateRelic(RelicSO relic, int price)
        {
            return new ShopItemData
            {
                Type = ShopItemType.Relic,
                Relic = relic,
                Price = price
            };
        }
    }
}