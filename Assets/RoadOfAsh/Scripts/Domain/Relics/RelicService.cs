using System;
using RoadOfAsh.Scripts.Domain.Players;

namespace RoadOfAsh.Scripts.Domain.Relics
{
    public class RelicService : IRelicService
    {
        private readonly PlayerState _playerState;
        
        private bool _distortionBlockedThisTurn;

        public event Action<RelicSO> RelicActivated;

        public RelicService(PlayerState playerState)
        {
            _playerState = playerState;
        }

        public bool HasRelic(RelicEffectType effectType)
        {
            if (_playerState.Relics == null)
                return false;

            foreach (RelicSO relic in _playerState.Relics)
            {
                if (relic != null && relic.EffectType == effectType)
                    return true;
            }

            return false;
        }

        public int GetTotalValue(RelicEffectType effectType)
        {
            if (_playerState.Relics == null)
                return 0;

            int total = 0;

            foreach (RelicSO relic in _playerState.Relics)
            {
                if (relic == null)
                    continue;

                if (relic.EffectType == effectType)
                    total += relic.Value;
            }

            return total;
        }

        public void AddRelic(RelicSO relic)
        {
            if (relic == null)
                return;

            if (!_playerState.Relics.Contains(relic))
                _playerState.Relics.Add(relic);
        }

        public void ResetBattleRelicState()
        {
            _distortionBlockedThisTurn = false;
        }

        public bool TryBlockDistortion()
        {
            if (_distortionBlockedThisTurn)
                return false;

            RelicSO relic = FindRelic(RelicEffectType.BlockFirstDistortionEachTurn);

            if (relic == null)
                return false;

            _distortionBlockedThisTurn = true;
            RelicActivated?.Invoke(relic);

            return true;
        }
        
        public int GetBlockAtTurnStart()
        {
            return GetTotalValue(RelicEffectType.GainBlockAtTurnStart);
        }

        public int GetGoldAfterBattle()
        {
            return GetTotalValue(RelicEffectType.GainGoldAfterBattle);
        }

        public int GetHealAfterBattle()
        {
            return GetTotalValue(RelicEffectType.HealAfterBattle);
        }
        
        private RelicSO FindRelic(RelicEffectType effectType)
        {
            if (_playerState.Relics == null)
                return null;

            foreach (RelicSO relic in _playerState.Relics)
            {
                if (relic != null && relic.EffectType == effectType)
                    return relic;
            }

            return null;
        }
    }
}