using System;
using RoadOfAsh.Scripts.Domain.Cards;

namespace RoadOfAsh.Scripts.Domain.Battle
{
    public interface IBattleService
    {
        event Action OnBattleStateChanged;
        event Action<CardSO, PlayedCardResult> OnCardPlayed;

        bool IsBattleFinished { get; }
        bool PlayerWon { get; }
        EnemyState CurrentEnemy { get; }

        void StartBattle(EnemyState enemy);
        bool TryPlayCard(CardSO card);
        void EndPlayerTurn();
    }
}