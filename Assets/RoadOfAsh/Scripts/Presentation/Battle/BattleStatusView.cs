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

        public void Refresh(PlayerState player, EnemyState enemy)
        {
            RebuildPlayerStatuses(player);
            RebuildEnemyStatuses(enemy);
        }

        private void RebuildPlayerStatuses(PlayerState player)
        {
            Clear(playerStatusRoot);

            if (player.Poison > 0)
                Add(playerStatusRoot, poisonIcon, player.Poison, "Яд", "В конце хода получает урон.");

            if (player.Weak > 0)
                Add(playerStatusRoot, weakIcon, player.Weak, "Слабость", "Урон атак снижен на 25%.");
        }

        private void RebuildEnemyStatuses(EnemyState enemy)
        {
            Clear(enemyStatusRoot);

            if (enemy == null)
                return;

            if (enemy.Poison > 0)
                Add(enemyStatusRoot, poisonIcon, enemy.Poison, "Яд", "В конце хода получает урон.");

            if (enemy.Weak > 0)
                Add(enemyStatusRoot, weakIcon, enemy.Weak, "Слабость", "Урон атак снижен на 25%.");
        }

        private void Add(Transform root, Sprite icon, int value, string title, string description)
        {
            StatusIconView view = Instantiate(statusIconPrefab, root);
            view.Setup(icon, value, title, description, tooltipSystem);
        }

        private void Clear(Transform root)
        {
            for (int i = root.childCount - 1; i >= 0; i--)
                Destroy(root.GetChild(i).gameObject);
        }
    }
}