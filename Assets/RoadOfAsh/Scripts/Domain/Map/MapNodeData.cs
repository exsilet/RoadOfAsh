using System;
using RoadOfAsh.Scripts.Domain.Battle;
using RoadOfAsh.Scripts.Domain.Events;
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
        public EnemySO Enemy;
        public EventSO Event;

        public MapNodeData(int id, MapNodeType type, Vector2 position, int[] nextNodeIds, EnemySO enemy, EventSO eventData)
        {
            Id = id;
            Type = type;
            Position = position;
            NextNodeIds = nextNodeIds;
            Enemy = enemy;
            Event = eventData;
        }
    }
}