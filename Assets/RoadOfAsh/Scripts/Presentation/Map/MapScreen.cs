using RoadOfAsh.Scripts.Domain;
using RoadOfAsh.Scripts.Domain.Map;
using RoadOfAsh.Scripts.Domain.Players;
using RoadOfAsh.Scripts.Domain.Rewards;
using RoadOfAsh.Scripts.Presentation.Rewards;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace RoadOfAsh.Scripts.Presentation.Map
{
    public class MapScreen : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private RectTransform nodesRoot;
        [SerializeField] private MapNodeView nodePrefab;
        [SerializeField] private CampfireView campfireView;
        [SerializeField] private float campfireHealPercent = 0.3f;
        [SerializeField] private RewardSelectionView rewardSelectionView;
        
        [SerializeField] private MapSO mapConfig;
        
        [Header("Scenes")]
        [SerializeField] private string battleSceneName = "BattleScene";

        private IMapService _mapService;
        private PlayerState _playerState;
        private RewardService _rewardService;
        private RunState _runState;

        [Inject]
        public void Construct(IMapService mapService, PlayerState playerState, RewardService rewardService, RunState runState)
        {
            _mapService = mapService;
            _playerState = playerState;
            _rewardService = rewardService;
            _runState = runState;
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
            
            if (campfireView != null)
            {
                campfireView.Hide();
                campfireView.HealClicked += OnCampfireHealClicked;
                campfireView.CloseClicked += OnCampfireCloseClicked;
            }
            
            if (rewardSelectionView != null)
            {
                rewardSelectionView.Initialize(
                    _rewardService,
                    OnTreasureRewardSelected,
                    OnTreasureRewardSkipped);
            }

            if (_mapService.State == null)
                _mapService.CreateNewMap(mapConfig);

            BuildMap();
        }
        
        private void OnDestroy()
        {
            if (campfireView != null)
            {
                campfireView.HealClicked -= OnCampfireHealClicked;
                campfireView.CloseClicked -= OnCampfireCloseClicked;
            }
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
                    OpenCampfire();
                    break;

                case MapNodeType.Treasure:
                    OpenTreasure();
                    break;
            }
        }
        
        private void OpenCampfire()
        {
            if (_playerState == null)
            {
                Debug.LogError("MapScreen: PlayerState is NULL.");
                return;
            }

            int healAmount = Mathf.RoundToInt(_playerState.MaxHP * campfireHealPercent);

            if (campfireView != null)
                campfireView.Show(healAmount);
        }
        
        private void OnCampfireHealClicked()
        {
            if (_playerState == null || _mapService == null)
                return;

            int healAmount = Mathf.RoundToInt(_playerState.MaxHP * campfireHealPercent);

            _playerState.Heal(healAmount);
            _mapService.CompleteSelectedNode();

            if (campfireView != null)
                campfireView.Hide();

            BuildMap();
        }
        
        private void OnCampfireCloseClicked()
        {
            if (campfireView != null)
                campfireView.Hide();
        }
        
        private void OpenTreasure()
        {
            if (rewardSelectionView != null)
                rewardSelectionView.Show();
        }
        
        private void OnTreasureRewardSelected(RewardItem reward)
        {
            if (reward == null)
                return;

            switch (reward.Type)
            {
                case RewardType.Card:
                    if (reward.Card != null)
                        _playerState.Deck.Add(reward.Card);
                    break;

                case RewardType.Gold:
                    _runState.AddGold(reward.Amount);
                    break;

                case RewardType.Heal:
                    _playerState.Heal(reward.Amount);
                    break;
            }

            CompleteTreasureNode();
        }
        
        private void OnTreasureRewardSkipped()
        {
            CompleteTreasureNode();
        }
        
        private void CompleteTreasureNode()
        {
            if (_mapService != null)
                _mapService.CompleteSelectedNode();

            if (rewardSelectionView != null)
                rewardSelectionView.Hide();

            BuildMap();
        }
    }
}