using System;
using UnityEngine;

namespace RoadOfAsh.Scripts.Domain.Map
{
    [Serializable]
    public class MapNodeData
    {
        public int Id;
        public MapNodeType Type;
        public Vector2 Position;
        public int[] NextNodeIds;

        public MapNodeData(int id, MapNodeType type, Vector2 position, int[] nextNodeIds)
        {
            Id = id;
            Type = type;
            Position = position;
            NextNodeIds = nextNodeIds;
        }
    }
}