using System;
using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Events;
using RoadOfAsh.Scripts.Domain.Map;
using RoadOfAsh.Scripts.Domain.Players;
using RoadOfAsh.Scripts.Domain.Relics;
using RoadOfAsh.Scripts.Domain.Rewards;
using RoadOfAsh.Scripts.Domain.Shop;
using RoadOfAsh.Scripts.Infrastructure;
using RoadOfAsh.Scripts.Infrastructure.Saves;
using RoadOfAsh.Scripts.Presentation.Rewards;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace RoadOfAsh.Scripts.Presentation.Map
{
    public class MapScreen : MonoBehaviour
    {
        [Header("UI")] [SerializeField] private RectTransform nodesRoot;
        [SerializeField] private MapNodeView nodePrefab;
        [SerializeField] private CampfireView campfireView;
        [SerializeField] private float campfireHealPercent = 0.3f;
        [SerializeField] private RewardSelectionView rewardSelectionView;
        [SerializeField] private EventView eventView;
        [SerializeField] private CardUpgradeSelectionView cardUpgradeSelectionView;

        [SerializeField] private MapSO mapConfig;
        [SerializeField] private MapShopFlow mapShopFlow;

        [Header("Scenes")] [SerializeField] private string battleSceneName = "BattleScene";

        private PlayerState _playerState;
        private RunState _runState;
        private IShopService _shopService;
        private IMapService _mapService;
        private IRewardService _rewardService;
        private CardUpgradeService _cardUpgradeService;
        private IRelicService _relicService;
        private ISaveService _saveService;

        [Inject]
        public void Construct(IMapService mapService, PlayerState playerState, IRewardService rewardService, RunState runState, IShopService shopService,
            CardUpgradeService cardUpgradeService, IRelicService relicService, ISaveService saveService)
        {
            _mapService = mapService;
            _playerState = playerState;
            _rewardService = rewardService;
            _runState = runState;
            _shopService = shopService;
            _cardUpgradeService = cardUpgradeService;
            _relicService = relicService;
            _saveService = saveService;
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

            bool shouldCreateNewMap = RunStartMode.ForceNewMap || _mapService.State == null;

            if (shouldCreateNewMap)
            {
                if (RunStartMode.ForceNewMap)
                {
                    Debug.Log("MAP START: creating new map by force.");

                    RunStartMode.ForceNewMap = false;
                    _mapService.CreateNewMap(mapConfig);
                }
                else if (_saveService != null && _saveService.TryRestoreMap(mapConfig))
                {
                    Debug.Log("MAP START: restored map from save.");
                }
                else
                {
                    Debug.Log("MAP START: creating new map.");

                    _mapService.CreateNewMap(mapConfig);
                }
            }

            if (campfireView != null)
            {
                campfireView.Hide();
                campfireView.HealClicked += OnCampfireHealClicked;
                campfireView.CloseClicked += OnCampfireCloseClicked;
                campfireView.UpgradeClicked += OnCampfireUpgradeClicked;
            }

            if (rewardSelectionView != null)
            {
                rewardSelectionView.Initialize(
                    _rewardService,
                    OnTreasureRewardSelected,
                    OnTreasureRewardSkipped);
            }

            if (eventView != null)
            {
                eventView.Hide();
                eventView.ChoiceClicked += OnEventChoiceClicked;
            }

            if (mapShopFlow != null)
            {
                mapShopFlow.Initialize(_mapService, _playerState, _runState, _shopService, _relicService, _saveService);
                mapShopFlow.ShopCompleted += OnShopCompleted;
            }

            Debug.Log(
                $"MAP START READY: Current = {_mapService.State.CurrentNodeId}, Selected = {_mapService.State.SelectedNodeId}, Completed = {_mapService.State.CompletedNodeIds.Count}");
            
            BuildMap();
        }

        private void OnDestroy()
        {
            if (campfireView != null)
            {
                campfireView.HealClicked -= OnCampfireHealClicked;
                campfireView.CloseClicked -= OnCampfireCloseClicked;
                campfireView.UpgradeClicked -= OnCampfireUpgradeClicked;
            }

            if (eventView != null)
                eventView.ChoiceClicked -= OnEventChoiceClicked;

            if (mapShopFlow != null)
                mapShopFlow.ShopCompleted -= OnShopCompleted;
        }

        private void OnShopCompleted()
        {
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
                    RunLifetimeScope.LoadScene(battleSceneName);
                    break;

                case MapNodeType.Event:
                    OpenEvent(selectedNode);
                    break;

                case MapNodeType.Shop:
                    mapShopFlow.Open();
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
            _saveService?.SaveRun();

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
            {
                _mapService.CompleteSelectedNode();
                _saveService?.SaveRun();
            }

            if (rewardSelectionView != null)
                rewardSelectionView.Hide();

            BuildMap();
        }

        private void OpenEvent(MapNodeData node)
        {
            if (node == null || node.Event == null)
            {
                Debug.LogError("MapScreen: Event node has no EventSO.");
                return;
            }

            if (eventView != null)
                eventView.Show(node.Event);
        }

        private void OnEventChoiceClicked(EventChoiceData choice)
        {
            if (choice == null)
                return;

            if (choice.HpCost > 0)
                _playerState.HP = Mathf.Max(1, _playerState.HP - choice.HpCost);

            switch (choice.Type)
            {
                case EventChoiceType.None:
                    break;

                case EventChoiceType.GainGold:
                    _runState.AddGold(choice.Amount);
                    break;

                case EventChoiceType.Heal:
                    _playerState.Heal(choice.Amount);
                    break;

                case EventChoiceType.LoseHp:
                    _playerState.HP = Mathf.Max(1, _playerState.HP - choice.Amount);
                    break;

                case EventChoiceType.GainCard:
                    if (choice.Card != null)
                        _playerState.Deck.Add(choice.Card);
                    break;
            }

            if (eventView != null)
                eventView.Hide();

            _mapService.CompleteSelectedNode();
            _saveService?.SaveRun();
            BuildMap();
        }

        private void OnCampfireUpgradeClicked()
        {
            if (_playerState == null || _cardUpgradeService == null)
                return;

            if (!_cardUpgradeService.CanUpgradeAnyCard())
            {
                Debug.Log("MapScreen: no cards available for upgrade.");
                return;
            }

            if (campfireView != null)
                campfireView.Hide();

            if (cardUpgradeSelectionView != null)
                cardUpgradeSelectionView.Show(_playerState.Deck, OnCardSelectedForUpgrade);
        }

        private void OnCardSelectedForUpgrade(CardSO card)
        {
            if (_cardUpgradeService == null)
                return;

            bool upgraded = _cardUpgradeService.TryUpgradeCard(card);

            if (!upgraded)
                return;

            if (cardUpgradeSelectionView != null)
                cardUpgradeSelectionView.Hide();

            if (_mapService != null)
            {
                _mapService.CompleteSelectedNode();
                _saveService?.SaveRun();
            }

            BuildMap();
        }
    }
}