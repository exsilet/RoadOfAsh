namespace RoadOfAsh.Scripts.Domain.Battle
{
    public class EnemyState
    {
        public string Name { get; set; }
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int Block { get; set; }
        public int Damage { get; set; }
        public int Weak { get; set; }
        public int Poison { get; set; }
        public EnemyIntentType IntentType { get; set; }
        public int IntentValue { get; set; }
        public int TurnIndex { get; set; }
        public EnemyIntentStep[] Pattern { get; set; }
    }
}