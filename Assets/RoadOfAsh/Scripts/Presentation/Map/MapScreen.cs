using RoadOfAsh.Scripts.Domain.Map;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using VContainer;

namespace RoadOfAsh.Scripts.Presentation.Map
{
    public class MapScreen : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private RectTransform nodesRoot;
        [SerializeField] private MapNodeView nodePrefab;
        
        [SerializeField] private MapSO mapConfig;
        
        [Header("Scenes")]
        [SerializeField] private string battleSceneName = "BattleScene";

        private IMapService _mapService;

        [Inject]
        public void Construct(IMapService mapService)
        {
            _mapService = mapService;
        }

        private void Start()
        {
            if (_mapService == null)
            {
                Debug.LogError("MapScreen: MapService is NULL. Check MapLifetimeScope parent and registration.");
                return;
            }

            if (mapConfig == null)
            {
                Debug.LogError("MapScreen: Map Config is NULL.");
                return;
            }

            if (nodesRoot == null)
            {
                Debug.LogError("MapScreen: Nodes Root is NULL.");
                return;
            }

            if (nodePrefab == null)
            {
                Debug.LogError("MapScreen: Node Prefab is NULL.");
                return;
            }

            if (_mapService.State == null)
                _mapService.CreateNewMap(mapConfig);

            BuildMap();
        }

        private void BuildMap()
        {
            foreach (Transform child in nodesRoot)
                Destroy(child.gameObject);

            foreach (var node in _mapService.GetNodes())
            {
                var view = Instantiate(nodePrefab, nodesRoot);
                view.GetComponent<RectTransform>().anchoredPosition = node.Position;

                var state = _mapService.GetNodeState(node.Id);
                view.Setup(node, state, this);
            }
        }

        public void OnNodeClicked(int nodeId)
        {
            if (!_mapService.TrySelectNode(nodeId))
                return;

            var selectedNode = _mapService.GetSelectedNode();

            switch (selectedNode.Type)
            {
                case MapNodeType.Battle:
                case MapNodeType.EliteBattle:
                case MapNodeType.Boss:
                    SceneManager.LoadScene(battleSceneName);
                    break;

                case MapNodeType.Event:
                    Debug.Log("Event пока не сделан");
                    break;

                case MapNodeType.Shop:
                    Debug.Log("Shop пока не сделан");
                    break;

                case MapNodeType.Campfire:
                    Debug.Log("Campfire пока не сделан");
                    break;

                case MapNodeType.Treasure:
                    Debug.Log("Treasure пока не сделан");
                    break;
            }
        }
    }
}