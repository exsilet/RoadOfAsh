using System;
using RoadOfAsh.Scripts.Domain.Battle;
using UnityEngine;

namespace RoadOfAsh.Scripts.Domain.Map
{
    [Serializable]
    public class MapNodeConfig
    {
        public int Id;
        public MapNodeType Type;
        public Vector2 Position;
        public int[] NextNodeIds;
        public EnemySO Enemy;
    }
}