using System.Collections.Generic;
using System.Linq;

namespace RoadOfAsh.Scripts.Domain.Map
{
    public class MapService : IMapService
    {
        public MapState State { get; private set; }

        public void CreateNewMap(MapSO mapConfig)
        {
            State = new MapState(mapConfig.Nodes, mapConfig.StartNodeId);
        }

        public IReadOnlyList<MapNodeData> GetNodes()
        {
            EnsureState();
            return State.Nodes;
        }

        public MapNodeState GetNodeState(int nodeId)
        {
            EnsureState();

            if (State.CompletedNodeIds.Contains(nodeId))
                return MapNodeState.Completed;

            if (State.CurrentNodeId == nodeId)
                return MapNodeState.Current;

            if (CanSelectNode(nodeId))
                return MapNodeState.Available;

            return MapNodeState.Locked;
        }

        public bool CanSelectNode(int nodeId)
        {
            EnsureState();

            var currentNode = State.Nodes.FirstOrDefault(n => n.Id == State.CurrentNodeId);

            if (currentNode == null)
                return false;

            return currentNode.NextNodeIds.Contains(nodeId);
        }

        public bool TrySelectNode(int nodeId)
        {
            if (!CanSelectNode(nodeId))
                return false;

            State.SelectNode(nodeId);
            return true;
        }

        public void CompleteSelectedNode()
        {
            EnsureState();
            State.CompleteSelectedNode();
        }

        public MapNodeData GetSelectedNode()
        {
            EnsureState();

            if (State.SelectedNodeId < 0)
                return null;

            return State.Nodes.FirstOrDefault(n => n.Id == State.SelectedNodeId);
        }

        private void EnsureState()
        {
            if (State == null)
                    throw new System.InvalidOperationException(
                        "MapState is null. Call CreateNewMap(mapConfig) before using MapService."
                    );
        }
    }
}