using UnityEngine;

namespace RoadOfAsh.Scripts.Domain.Battle
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "Road Of Ash/Battle/Enemy")]
    public class EnemySO : ScriptableObject
    {
        [SerializeField] private string enemyName = "Баба-Яга";
        [SerializeField] private int maxHp = 40;
        [SerializeField] private int damage = 6;
        [SerializeField] private EnemyIntentStep[] pattern;

        public string EnemyName => enemyName;
        public int MaxHp => maxHp;
        public int Damage => damage;
        public EnemyIntentStep[] Pattern => pattern;

        public EnemyState CreateState()
        {
            return new EnemyState
            {
                Name = enemyName,
                HP = maxHp,
                MaxHP = maxHp,
                Damage = damage,
                Block = 0,
                Weak = 0,
                Poison = 0,
                TurnIndex = 0,
                Pattern = pattern
            };
        }
    }
}