namespace RoadOfAsh.Scripts.Domain.Battle
{
    public class BattleState
    {
        public int Turn { get; set; } = 1;
        public bool IsPlayerTurn { get; set; } = true;
    }
}