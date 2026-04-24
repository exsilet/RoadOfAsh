using System;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Players;
using UnityEngine;

namespace RoadOfAsh.Scripts.Domain.Battle
{
    public class BattleService : IBattleService
    {
        private readonly PlayerState _playerState;
        private readonly ICardService _cardService;

        private EnemyState _enemy;
        private bool _finished;
        private bool _playerWon;

        public event Action OnBattleStateChanged;
        public event Action<CardSO, PlayedCardResult> OnCardPlayed;

        public bool IsBattleFinished => _finished;
        public bool PlayerWon => _playerWon;
        public EnemyState CurrentEnemy => _enemy;

        public BattleService(PlayerState playerState, ICardService cardService)
        {
            _playerState = playerState;
            _cardService = cardService;
        }

        public void StartBattle(EnemyState enemy)
        {
            _enemy = enemy;
            _finished = false;
            _playerWon = false;

            _playerState.Block = 0;
            _playerState.Energy = 3;
            _playerState.Weak = 0;
            _playerState.Poison = 0;

            _cardService.Draw(5);
            NotifyStateChanged();
        }

        public bool TryPlayCard(CardSO card)
        {
            if (_finished) return false;
            if (!_cardService.TryPlayCard(card)) return false;

            var result = BuildCardResult(card);

            ApplyPlayedCardResult(result);
            CheckBattleFinished();

            OnCardPlayed?.Invoke(card, result);
            NotifyStateChanged();

            return true;
        }

        public void EndPlayerTurn()
        {
            if (_finished) return;
            if (_enemy == null) return;

            _cardService.DiscardHand();

            ApplyPoisonToEnemy();

            bool enemyDiedFromPoison = _enemy.HP <= 0;
            if (enemyDiedFromPoison)
            {
                _enemy.HP = 0;
                _finished = true;
                _playerWon = true;
            }

            int incomingDamage = 0;

            if (!enemyDiedFromPoison)
            {
                incomingDamage = _enemy.Damage;

                if (_enemy.Weak > 0)
                {
                    incomingDamage = Mathf.Max(0, Mathf.RoundToInt(incomingDamage * 0.75f));
                    _enemy.Weak--;
                }

                int blockedDamage = Mathf.Min(_playerState.Block, incomingDamage);
                int finalDamage = incomingDamage - blockedDamage;

                _playerState.Block -= blockedDamage;
                _playerState.HP -= finalDamage;

                if (_playerState.HP <= 0)
                {
                    _playerState.HP = 0;
                    _finished = true;
                    _playerWon = false;
                }
            }

            _playerState.Block = 0;
            _playerState.Energy = 3;

            _cardService.Draw(5);

            NotifyStateChanged();
        }

        private PlayedCardResult BuildCardResult(CardSO card)
        {
            var result = new PlayedCardResult
            {
                WasCorrupted = false,
                FinalEffects = card.Effects,
                FinalCost = card.Cost
            };

            if (!card.CanBeCorrupted)
                return result;

            float roll = UnityEngine.Random.value;

            if (roll <= card.CorruptionChance && card.CorruptedEffects.Count > 0)
            {
                result.WasCorrupted = true;
                result.FinalEffects = card.CorruptedEffects;
                result.FinalCost = card.CorruptedCost > 0 ? card.CorruptedCost : card.Cost;
            }

            return result;
        }

        private void ApplyPlayedCardResult(PlayedCardResult result)
        {
            foreach (var effect in result.FinalEffects)
            {
                switch (effect.Type)
                {
                    case EffectType.Damage:
                        _enemy.HP -= effect.Value;
                        break;

                    case EffectType.Block:
                        _playerState.Block += effect.Value;
                        break;

                    case EffectType.Draw:
                        _cardService.Draw(effect.Value);
                        break;

                    case EffectType.ApplyWeak:
                        _enemy.Weak += effect.Value;
                        break;

                    case EffectType.ApplyPoison:
                        _enemy.Poison += effect.Value;
                        break;

                    case EffectType.GainEnergy:
                        _playerState.Energy += effect.Value;
                        break;
                }
            }
        }

        private void ApplyPoisonToEnemy()
        {
            if (_enemy.Poison <= 0)
                return;

            _enemy.HP -= _enemy.Poison;
        }

        private void CheckBattleFinished()
        {
            if (_enemy.HP <= 0)
            {
                _enemy.HP = 0;
                _finished = true;
                _playerWon = true;
            }
        }

        private void NotifyStateChanged()
        {
            OnBattleStateChanged?.Invoke();
        }
    }
}