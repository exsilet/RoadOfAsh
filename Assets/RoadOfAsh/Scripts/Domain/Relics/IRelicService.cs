using System;

namespace RoadOfAsh.Scripts.Domain.Relics
{
    public interface IRelicService
    {
        event Action<RelicSO> RelicActivated;
        bool HasRelic(RelicEffectType effectType);
        int GetTotalValue(RelicEffectType effectType);
        
        int GetBlockAtTurnStart();
        int GetGoldAfterBattle();
        int GetHealAfterBattle();

        void AddRelic(RelicSO relic);
        void ResetBattleRelicState();

        bool TryBlockDistortion();
    }
}