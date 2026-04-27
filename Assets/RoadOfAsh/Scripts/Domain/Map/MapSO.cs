using System.Collections.Generic;
using UnityEngine;

namespace RoadOfAsh.Scripts.Domain.Map
{
    [CreateAssetMenu(menuName = "Road Of Ash/Map/Map Config")]
    public class MapSO : ScriptableObject
    {
        public int StartNodeId = 0;
        public List<MapNodeData> Nodes = new();
    }
}