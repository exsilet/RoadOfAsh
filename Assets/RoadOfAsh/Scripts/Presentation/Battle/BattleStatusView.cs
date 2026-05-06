using RoadOfAsh.Scripts.Domain.Battle;
using RoadOfAsh.Scripts.Domain.Players;
using UnityEngine;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class BattleStatusView : MonoBehaviour
    {
        [SerializeField] private Transform playerStatusRoot;
        [SerializeField] private Transform enemyStatusRoot;
        [SerializeField] private StatusIconView statusIconPrefab;
        [SerializeField] private StatusTooltipSystem tooltipSystem;

        [Header("Icons")]
        [SerializeField] private Sprite poisonIcon;
        [SerializeField] private Sprite weakIcon;

        [Header("Poison Text")]
        [SerializeField] private string poisonTitle = "Яд";
        [SerializeField] private string poisonDescription = "В конце хода получает урон.";

        [Header("Weak Text")]
        [SerializeField] private string weakTitle = "Слабость";
        [SerializeField] private string weakDescription = "Урон атак снижен на 25%.";

        public void Refresh(PlayerState player, EnemyState enemy)
        {
            RebuildPlayerStatuses(player);
            RebuildEnemyStatuses(enemy);
        }

        private void RebuildPlayerStatuses(PlayerState player)
        {
            Clear(playerStatusRoot);

            if (player == null)
                return;

            if (player.Poison > 0)
                Add(playerStatusRoot, poisonIcon, player.Poison, poisonTitle, poisonDescription);

            if (player.Weak > 0)
                Add(playerStatusRoot, weakIcon, player.Weak, weakTitle, weakDescription);
        }

        private void RebuildEnemyStatuses(EnemyState enemy)
        {
            Clear(enemyStatusRoot);

            if (enemy == null)
                return;

            if (enemy.Poison > 0)
                Add(enemyStatusRoot, poisonIcon, enemy.Poison, poisonTitle, poisonDescription);

            if (enemy.Weak > 0)
                Add(enemyStatusRoot, weakIcon, enemy.Weak, weakTitle, weakDescription);
        }

        private void Add(Transform root, Sprite icon, int value, string title, string description)
        {
            if (root == null || statusIconPrefab == null)
                return;

            StatusIconView view = Instantiate(statusIconPrefab, root);
            view.Setup(icon, value, title, description, tooltipSystem);
        }

        private void Clear(Transform root)
        {
            if (root == null)
                return;

            for (int i = root.childCount - 1; i >= 0; i--)
                Destroy(root.GetChild(i).gameObject);
        }
    }
}