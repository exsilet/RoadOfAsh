using System;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Distortion;
using RoadOfAsh.Scripts.Domain.Players;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RoadOfAsh.Scripts.Domain.Battle
{
    public class BattleService : IBattleService
    {
        private readonly PlayerState _playerState;
        private readonly ICardService _cardService;
        private readonly IDistortionService _distortionService;

        private EnemyState _enemy;
        private bool _finished;
        private bool _playerWon;

        public event Action OnBattleStateChanged;
        public event Action<CardSO, PlayedCardResult> OnCardPlayed;
        public event Action<int> OnEnemyDamaged;
        public event Action<int> OnPlayerDamaged;
        public event Action<int> OnPlayerBlocked;
        public event Action<int> OnEnemyPoisonTick;

        public bool IsBattleFinished => _finished;
        public bool PlayerWon => _playerWon;
        public EnemyState CurrentEnemy => _enemy;

        public BattleService(PlayerState playerState, ICardService cardService, IDistortionService distortionService)
        {
            _playerState = playerState;
            _cardService = cardService;
            _distortionService = distortionService;
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
            _enemy.Block = 0;

            _cardService.Draw(5);
            RollEnemyIntent();
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
                FinishBattle(true);
            }

            if (!enemyDiedFromPoison)
            {
                _enemy.Block = 0;
                ExecuteEnemyIntent();
                _enemy.TurnIndex++;
                
                _enemy.Weak = 0;
            }

            _playerState.Block = 0;
            _playerState.Energy = 3;

            _cardService.Draw(5);
            
            if (!_finished)
                RollEnemyIntent();

            NotifyStateChanged();
        }

        private PlayedCardResult BuildCardResult(CardSO card)
        {
            return _distortionService.Resolve(card);
        }

        private void ApplyPlayedCardResult(PlayedCardResult result)
        {
            foreach (var effect in result.FinalEffects)
            {
                switch (effect.Type)
                {
                    case EffectType.Damage:
                        ApplyDamageToEnemy(effect.Value);
                        break;

                    case EffectType.Block:
                        _playerState.Block += effect.Value;
                        OnPlayerBlocked?.Invoke(effect.Value);
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
        
        private void ApplyDamageToEnemy(int damage)
        {
            if (damage <= 0)
                return;

            int blockedDamage = Mathf.Min(_enemy.Block, damage);
            int finalDamage = damage - blockedDamage;

            _enemy.Block -= blockedDamage;
            _enemy.HP -= finalDamage;

            if (finalDamage > 0)
                OnEnemyDamaged?.Invoke(finalDamage);
        }

        private void ApplyPoisonToEnemy()
        {
            if (_enemy.Poison <= 0)
                return;

            int poisonDamage = _enemy.Poison;

            _enemy.HP = Mathf.Max(0, _enemy.HP - poisonDamage);
            OnEnemyPoisonTick?.Invoke(poisonDamage);

            _enemy.Poison = 0;
        }
        
        private void RollEnemyIntent()
        {
            if (_enemy.Pattern == null || _enemy.Pattern.Length == 0)
            {
                _enemy.IntentType = EnemyIntentType.Attack;
                _enemy.IntentValue = _enemy.Damage;
                return;
            }

            int index = _enemy.TurnIndex % _enemy.Pattern.Length;
            EnemyIntentStep step = _enemy.Pattern[index];

            _enemy.IntentType = step.Type;
            _enemy.IntentValue = step.Value;
        }
        
        private void ExecuteEnemyIntent()
        {
            switch (_enemy.IntentType)
            {
                case EnemyIntentType.Attack:
                    ApplyEnemyAttack(_enemy.IntentValue);
                    break;

                case EnemyIntentType.Block:
                    _enemy.Block += _enemy.IntentValue;
                    break;

                case EnemyIntentType.Buff:
                    _enemy.Weak = Mathf.Max(0, _enemy.Weak - _enemy.IntentValue);
                    break;
            }
        }
        
        private void ApplyEnemyAttack(int damage)
        {
            int incomingDamage = damage;

            if (_enemy.Weak > 0)
            {
                incomingDamage = Mathf.Max(0, Mathf.RoundToInt(incomingDamage * 0.75f));
            }

            int blockedDamage = Mathf.Min(_playerState.Block, incomingDamage);
            int finalDamage = incomingDamage - blockedDamage;

            _playerState.Block -= blockedDamage;
            _playerState.HP -= finalDamage;
            
            if (finalDamage > 0)
                OnPlayerDamaged?.Invoke(finalDamage);

            if (_playerState.HP <= 0)
            {
                _playerState.HP = 0;
                _finished = true;
                _playerWon = false;
            }
        }

        private void CheckBattleFinished()
        {
            if (_enemy.HP <= 0)
            {
                _enemy.HP = 0;
                FinishBattle(true);
            }
        }
        
        private void FinishBattle(bool playerWon)
        {
            if (_finished)
                return;

            _finished = true;
            _playerWon = playerWon;

            if (playerWon)
                _distortionService.GainUnderstanding();
        }

        private void NotifyStateChanged()
        {
            OnBattleStateChanged?.Invoke();
        }
    }
}