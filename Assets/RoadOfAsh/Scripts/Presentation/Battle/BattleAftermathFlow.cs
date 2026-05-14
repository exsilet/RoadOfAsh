using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain;
using RoadOfAsh.Scripts.Domain.Players;
using RoadOfAsh.Scripts.Domain.Relics;
using UnityEngine;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class BattleAftermathFlow : MonoBehaviour
    {
        private readonly List<RelicSO> _relicsAtBattleStart = new();

        private PlayerState _playerState;
        private RunState _runState;

        public void Initialize(PlayerState playerState, RunState runState)
        {
            _playerState = playerState;
            _runState = runState;
        }

        public void CacheRelicsAtBattleStart()
        {
            _relicsAtBattleStart.Clear();

            if (_playerState == null || _playerState.Relics == null)
                return;

            foreach (RelicSO relic in _playerState.Relics)
            {
                if (relic == null)
                    continue;

                _relicsAtBattleStart.Add(relic);
            }
        }

        public void ApplyAfterBattleRelics(bool playerWon)
        {
            if (!playerWon)
                return;

            int gold = GetRelicValueAtBattleStart(RelicEffectType.GainGoldAfterBattle);
            if (gold > 0 && _runState != null)
                _runState.AddGold(gold);

            int heal = GetRelicValueAtBattleStart(RelicEffectType.HealAfterBattle);
            if (heal > 0 && _playerState != null)
                _playerState.Heal(heal);
        }

        private int GetRelicValueAtBattleStart(RelicEffectType effectType)
        {
            int total = 0;

            foreach (RelicSO relic in _relicsAtBattleStart)
            {
                if (relic == null)
                    continue;

                if (relic.EffectType == effectType)
                    total += relic.Value;
            }

            return total;
        }
    }
}