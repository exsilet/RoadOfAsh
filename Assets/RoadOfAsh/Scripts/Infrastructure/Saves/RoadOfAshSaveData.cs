using System;

namespace RoadOfAsh.Scripts.Infrastructure.Saves
{
    [Serializable]
    public class RoadOfAshSaveData
    {
        public bool HasSave;
        public bool IntroBattleCompleted;

        public int Gold;
        public int SkippedRewards;

        public int PlayerHp;
    }
}