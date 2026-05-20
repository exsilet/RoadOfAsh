using System;
using System.Collections.Generic;

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

        public int CurrentNodeId;
        public int SelectedNodeId = -1;
        public List<int> CompletedNodeIds = new();

        public List<string> DeckCardIds = new();
        public List<string> RelicIds = new();
    }
}