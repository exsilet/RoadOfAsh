namespace RoadOfAsh.Scripts.Domain
{
    public class RunState
    {
        public bool IntroBattleCompleted { get; set; }
        
        public int Gold { get; private set; }
        public int SkippedRewards { get; private set; }

        public void AddGold(int amount)
        {
            if (amount <= 0)
                return;

            Gold += amount;
        }

        public void AddSkippedReward()
        {
            SkippedRewards++;
            AddGold(15);
        }
        
        public bool SpendGold(int amount)
        {
            if (amount <= 0)
                return false;

            if (Gold < amount)
                return false;

            Gold -= amount;
            return true;
        }
    }
}