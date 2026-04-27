using RoadOfAsh.Scripts.Domain.Map;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoadOfAsh.Scripts.Presentation.Map
{
    public class MapNodeView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Button button;
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text labelText;

        [Header("Map Node Sprites")]
        [SerializeField] private Sprite playerSprite;
        [SerializeField] private Sprite battleSprite;
        [SerializeField] private Sprite eventSprite;
        [SerializeField] private Sprite campfireSprite;
        [SerializeField] private Sprite shopSprite;
        [SerializeField] private Sprite treasureSprite;
        [SerializeField] private Sprite eliteBattleSprite;
        [SerializeField] private Sprite bossSprite;

        [Header("State Visual")]
        [SerializeField] private Color currentColor = Color.white;
        [SerializeField] private Color availableColor = Color.white;
        [SerializeField] private Color completedColor = new Color(0.55f, 0.55f, 0.55f, 1f);
        [SerializeField] private Color lockedColor = new Color(0.25f, 0.25f, 0.25f, 0.65f);

        [SerializeField] private bool showLabel = false;

        private int _nodeId;
        private MapScreen _mapScreen;

        public void Setup(MapNodeData node, MapNodeState state, MapScreen mapScreen)
        {
            _nodeId = node.Id;
            _mapScreen = mapScreen;

            if (iconImage != null)
            {
                iconImage.sprite = GetSprite(node.Type);
                iconImage.preserveAspect = true;
            }

            if (labelText != null)
            {
                labelText.gameObject.SetActive(showLabel);
                labelText.text = GetLabel(node.Type);
            }

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnClick);
                button.interactable = state == MapNodeState.Available;
            }

            ApplyVisualState(state);
        }

        private void OnClick()
        {
            if (_mapScreen == null)
                return;

            _mapScreen.OnNodeClicked(_nodeId);
        }

        private void ApplyVisualState(MapNodeState state)
        {
            if (iconImage == null)
                return;

            iconImage.color = state switch
            {
                MapNodeState.Current => currentColor,
                MapNodeState.Available => availableColor,
                MapNodeState.Completed => completedColor,
                MapNodeState.Locked => lockedColor,
                _ => Color.white
            };

            transform.localScale = state switch
            {
                MapNodeState.Current => Vector3.one * 1.15f,
                MapNodeState.Available => Vector3.one * 1.08f,
                _ => Vector3.one
            };
        }

        private Sprite GetSprite(MapNodeType type)
        {
            return type switch
            {
                MapNodeType.Start => playerSprite,
                MapNodeType.Battle => battleSprite,
                MapNodeType.Event => eventSprite,
                MapNodeType.Shop => shopSprite,
                MapNodeType.Campfire => campfireSprite,
                MapNodeType.Treasure => treasureSprite,
                MapNodeType.EliteBattle => eliteBattleSprite,
                MapNodeType.Boss => bossSprite,
                _ => battleSprite
            };
        }

        private string GetLabel(MapNodeType type)
        {
            return type switch
            {
                MapNodeType.Start => "Игрок",
                MapNodeType.Battle => "Бой",
                MapNodeType.Event => "Событие",
                MapNodeType.Shop => "Магазин",
                MapNodeType.Campfire => "Костёр",
                MapNodeType.Treasure => "Сундук",
                MapNodeType.EliteBattle => "Элитный монстр",
                MapNodeType.Boss => "Босс",
                _ => "Узел"
            };
        }
    }
}