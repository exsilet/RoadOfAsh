using System;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Distortion;
using RoadOfAsh.Scripts.Domain.Players;
using RoadOfAsh.Scripts.Domain.Relics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RoadOfAsh.Scripts.Domain.Battle
{
    public class BattleService : IBattleService
    {
        private const float EnemyHpBonusPerUnderstanding = 0.03f;
        private const float EnemyIntentBonusPerUnderstanding = 0.02f;
        
        private readonly PlayerState _playerState;
        private readonly ICardService _cardService;
        private readonly IDistortionService _distortionService;
        private readonly IRelicService _relicService;

        private EnemyState _enemy;
        private bool _finished;
        private bool _playerWon;

        public event Action OnBattleStateChanged;
        public event Action OnPlayerTurnEnded;
        public event Action<CardSO, PlayedCardResult> OnCardPlayed;
        public event Action<int> OnEnemyDamaged;
        public event Action<int> OnPlayerDamaged;
        public event Action<int> OnPlayerBlocked;
        public event Action<int> OnEnemyPoisonTick;
        public event Action<int> OnPlayerPoisoned;
        public event Action<int> OnPlayerWeakened;
        public event Action<int> OnEnemyHealed;
        public event Action OnEnemyCleansed;

        public bool IsBattleFinished => _finished;
        public bool PlayerWon => _playerWon;
        public EnemyState CurrentEnemy => _enemy;

        public BattleService(PlayerState playerState, ICardService cardService, IDistortionService distortionService, IRelicService relicService)
        {
            _playerState = playerState;
            _cardService = cardService;
            _distortionService = distortionService;
            _relicService = relicService;
        }

        public void StartBattle(EnemyState enemy)
        {
            _enemy = enemy;
            _finished = false;
            _playerWon = false;

            ApplyUnderstandingDifficultyScaling();

            _playerState.Block = 0;
            _playerState.Energy = 3;
            
            ApplyRelicBlockAtTurnStart();
            
            _playerState.Weak = 0;
            _playerState.Poison = 0;
            _enemy.Block = 0;
            
            _distortionService.ResetTurnState();

            _cardService.Draw(5);
            RollEnemyIntent();
            NotifyStateChanged();
        }
        
        private void ApplyUnderstandingDifficultyScaling()
        {
            if (_enemy == null || _distortionService == null)
                return;

            int understanding = _distortionService.Understanding;

            if (understanding <= 0)
                return;

            float hpMultiplier = 1f + understanding * EnemyHpBonusPerUnderstanding;

            int oldMaxHp = _enemy.MaxHP;
            int newMaxHp = Mathf.Max(1, Mathf.RoundToInt(oldMaxHp * hpMultiplier));
            int hpBonus = newMaxHp - oldMaxHp;

            _enemy.MaxHP = newMaxHp;
            _enemy.HP = Mathf.Min(newMaxHp, _enemy.HP + hpBonus);
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
            _playerState.Weak = Mathf.Max(0, _playerState.Weak - 1);

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
                ApplyPoisonToPlayer();
                
                _enemy.TurnIndex++;
                _enemy.Weak = Mathf.Max(0, _enemy.Weak - 1);
            }

            _playerState.Block = 0;
            _playerState.Energy = 3;
            
            ApplyRelicBlockAtTurnStart();
            _distortionService.ResetTurnState();

            _cardService.Draw(5);
            
            if (!_finished)
                RollEnemyIntent();

            NotifyStateChanged();
            
            if (!_finished)
                OnPlayerTurnEnded?.Invoke();
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

            int incomingDamage = damage;

            if (_playerState.Weak > 0)
                incomingDamage = Mathf.Max(0, Mathf.RoundToInt(incomingDamage * 0.75f));

            int blockedDamage = Mathf.Min(_enemy.Block, incomingDamage);
            int finalDamage = incomingDamage - blockedDamage;

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

            _enemy.Poison = Mathf.Max(0, _enemy.Poison - 1);
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
                    ApplyEnemyAttack(ScaleEnemyIntentValue(_enemy.IntentValue));
                    break;
                case EnemyIntentType.Block:
                    _enemy.Block += ScaleEnemyIntentValue(_enemy.IntentValue);
                    break;
                case EnemyIntentType.HealSelf:
                    HealEnemy(ScaleEnemyIntentValue(_enemy.IntentValue));
                    break;
                case EnemyIntentType.Buff:
                    _enemy.Weak = Mathf.Max(0, _enemy.Weak - _enemy.IntentValue);
                    break;
                case EnemyIntentType.DistortNextCard:
                    _distortionService.ForceNextDistortion();
                    break;
                case EnemyIntentType.ApplyWeakToPlayer:
                    ApplyWeakToPlayer(_enemy.IntentValue);
                    break;
                case EnemyIntentType.ApplyPoisonToPlayer:
                    ApplyPoisonToPlayer(_enemy.IntentValue);
                    break;
                case EnemyIntentType.CleanseSelf:
                    CleanseEnemy();
                    break;
                default:
                    Debug.LogWarning($"Unhandled enemy intent: {_enemy.IntentType}");
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
        
        private void ApplyWeakToPlayer(int value)
        {
            if (value <= 0)
                return;

            _playerState.Weak += value;
            OnPlayerWeakened?.Invoke(value);
        }
        
        private void ApplyPoisonToPlayer(int value)
        {
            if (value <= 0)
                return;

            _playerState.Poison += value;
            OnPlayerPoisoned?.Invoke(value);
        }
        
        private void HealEnemy(int value)
        {
            if (value <= 0)
                return;

            int oldHp = _enemy.HP;
            _enemy.HP = Mathf.Min(_enemy.MaxHP, _enemy.HP + value);

            int healed = _enemy.HP - oldHp;

            if (healed > 0)
                OnEnemyHealed?.Invoke(healed);
        }

        private void CleanseEnemy()
        {
            bool hadStatuses = _enemy.Weak > 0 || _enemy.Poison > 0;

            _enemy.Weak = 0;
            _enemy.Poison = 0;

            if (hadStatuses)
                OnEnemyCleansed?.Invoke();
        }
        
        private void ApplyPoisonToPlayer()
        {
            if (_playerState.Poison <= 0)
                return;

            int poisonDamage = _playerState.Poison;
            _playerState.HP = Mathf.Max(0, _playerState.HP - poisonDamage);
            OnPlayerDamaged?.Invoke(poisonDamage);

            _playerState.Poison = Mathf.Max(0, _playerState.Poison - 1);

            if (_playerState.HP <= 0)
            {
                _playerState.HP = 0;
                _finished = true;
                _playerWon = false;
            }
        }
        
        private int ScaleEnemyIntentValue(int value)
        {
            if (value <= 0 || _distortionService == null)
                return value;

            int understanding = _distortionService.Understanding;

            if (understanding <= 0)
                return value;

            float multiplier = 1f + understanding * EnemyIntentBonusPerUnderstanding;

            return Mathf.Max(1, Mathf.RoundToInt(value * multiplier));
        }
        
        private void ApplyRelicBlockAtTurnStart()
        {
            if (_relicService == null)
                return;

            int block = _relicService.GetBlockAtTurnStart();

            if (block <= 0)
                return;

            _playerState.Block += block;
            OnPlayerBlocked?.Invoke(block);
        }
    }
}