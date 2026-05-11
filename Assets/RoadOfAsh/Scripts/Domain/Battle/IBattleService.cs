using System;
using RoadOfAsh.Scripts.Domain.Cards;

namespace RoadOfAsh.Scripts.Domain.Battle
{
    public interface IBattleService
    {
        event Action OnBattleStateChanged;
        event Action<CardSO, PlayedCardResult> OnCardPlayed;
        event Action<int> OnEnemyDamaged;
        event Action<int> OnPlayerDamaged;
        event Action<int> OnPlayerBlocked;
        event Action<int> OnEnemyPoisonTick;
        event Action<int> OnPlayerPoisoned;
        event Action<int> OnPlayerWeakened;
        event Action<int> OnEnemyHealed;
        event Action OnEnemyCleansed;
        bool IsBattleFinished { get; }
        bool PlayerWon { get; }
        EnemyState CurrentEnemy { get; }

        void StartBattle(EnemyState enemy);
        bool TryPlayCard(CardSO card);
        void EndPlayerTurn();
    }
}