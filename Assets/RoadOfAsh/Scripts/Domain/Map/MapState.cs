using System.Collections.Generic;

namespace RoadOfAsh.Scripts.Domain.Map
{
    public class MapState
    {
        public List<MapNodeData> Nodes { get; } = new();
        public HashSet<int> CompletedNodeIds { get; } = new();

        public int CurrentNodeId { get; private set; }
        public int SelectedNodeId { get; private set; } = -1;

        public MapState(List<MapNodeData> nodes, int startNodeId)
        {
            CurrentNodeId = startNodeId;

            foreach (MapNodeData node in nodes)
                Nodes.Add(new MapNodeData(node.Id, node.Type, node.Position, node.NextNodeIds, node.Enemy, node.Event));
        }

        public void SelectNode(int nodeId)
        {
            SelectedNodeId = nodeId;
        }

        public void CompleteSelectedNode()
        {
            if (SelectedNodeId < 0)
                return;

            CompletedNodeIds.Add(SelectedNodeId);
            CurrentNodeId = SelectedNodeId;
            SelectedNodeId = -1;
        }

        public void RestoreProgress(int currentNodeId, int selectedNodeId, IEnumerable<int> completedNodeIds)
        {
            CurrentNodeId = currentNodeId;
            SelectedNodeId = selectedNodeId;

            CompletedNodeIds.Clear();

            if (completedNodeIds == null)
                return;

            foreach (int nodeId in completedNodeIds)
                CompletedNodeIds.Add(nodeId);
        }
    }
}