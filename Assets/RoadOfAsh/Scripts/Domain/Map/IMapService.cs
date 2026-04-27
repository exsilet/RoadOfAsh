using System.Collections.Generic;

namespace RoadOfAsh.Scripts.Domain.Map
{
    public interface IMapService
    {
        MapState State { get; }
        void CreateNewMap(MapSO mapConfig);
        IReadOnlyList<MapNodeData> GetNodes();
        MapNodeState GetNodeState(int nodeId);
        bool CanSelectNode(int nodeId);
        bool TrySelectNode(int nodeId);
        void CompleteSelectedNode();
        MapNodeData GetSelectedNode();
    }
}