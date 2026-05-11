using RoadOfAsh.Scripts.Domain.Battle;
using RoadOfAsh.Scripts.Domain.Players;
using TMPro;
using UnityEngine;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class BattleHudView : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerHpText;
        [SerializeField] private TMP_Text enemyHpText;
        [SerializeField] private TMP_Text playerEnergyText;
        [SerializeField] private TMP_Text playerBlockText;
        [SerializeField] private TMP_Text enemyBlockText;
        [SerializeField] private TMP_Text enemyNameText;

        [Header("Enemy Intent")]
        [SerializeField] private EnemyIntentView enemyIntentView;
        [SerializeField] private StatusTooltipSystem tooltipSystem;

        [Header("Intent Icons")]
        [SerializeField] private Sprite attackIntentIcon;
        [SerializeField] private Sprite blockIntentIcon;
        [SerializeField] private Sprite buffIntentIcon;
        [SerializeField] private Sprite distortIntentIcon;
        [SerializeField] private Sprite weakIntentIcon;
        [SerializeField] private Sprite poisonIntentIcon;
        [SerializeField] private Sprite healIntentIcon;
        [SerializeField] private Sprite cleanseIntentIcon;

        [Header("Text Formats")]
        [SerializeField] private string enemyHpFormat = "HP: {0}/{1}";

        [Header("Intent Tooltip Titles")]
        [SerializeField] private string attackTitle = "Атака";
        [SerializeField] private string blockTitle = "Защита";
        [SerializeField] private string buffTitle = "Усиление";
        [SerializeField] private string distortTitle = "Искажение";
        [SerializeField] private string weakTitle = "Слабость";
        [SerializeField] private string poisonTitle = "Яд";
        [SerializeField] private string healTitle = "Лечение";
        [SerializeField] private string cleanseTitle = "Очищение";

        [Header("Intent Tooltip Descriptions")]
        [SerializeField] private string attackDescriptionFormat = "Атакует на {0}.";
        [SerializeField] private string blockDescriptionFormat = "Поставит {0} блока.";
        [SerializeField] private string buffDescription = "Усилится.";
        [SerializeField] private string distortDescription = "Следующая карта игрока будет искажена.";
        [SerializeField] private string weakDescriptionFormat = "Наложит {0} слабости.";
        [SerializeField] private string poisonDescriptionFormat = "Наложит {0} яда.";
        [SerializeField] private string healDescriptionFormat = "Восстановит {0} HP.";
        [SerializeField] private string cleanseDescription = "Снимет с себя яд и слабость.";

        public void Refresh(PlayerState playerState, EnemyState enemy)
        {
            if (playerState == null)
                return;

            if (playerHpText != null)
                playerHpText.text = $"{playerState.HP}/{playerState.MaxHP}";

            if (playerEnergyText != null)
                playerEnergyText.text = playerState.Energy.ToString();

            if (playerBlockText != null)
                playerBlockText.text = playerState.Block.ToString();

            if (enemy == null)
                return;

            if (enemyNameText != null)
                enemyNameText.text = enemy.Name;

            if (enemyHpText != null)
                enemyHpText.text = string.Format(enemyHpFormat, enemy.HP, enemy.MaxHP);

            if (enemyBlockText != null)
                enemyBlockText.text = enemy.Block.ToString();

            RefreshEnemyIntent(enemy);
        }

        private void RefreshEnemyIntent(EnemyState enemy)
        {
            if (enemyIntentView == null)
                return;

            Sprite icon = GetIntentIcon(enemy.IntentType);
            string title = GetIntentTitle(enemy.IntentType);
            string description = GetIntentDescription(enemy.IntentType, enemy.IntentValue);

            enemyIntentView.Setup(icon, title, description, tooltipSystem);
        }

        private Sprite GetIntentIcon(EnemyIntentType intentType)
        {
            return intentType switch
            {
                EnemyIntentType.Attack => attackIntentIcon,
                EnemyIntentType.Block => blockIntentIcon,
                EnemyIntentType.Buff => buffIntentIcon,
                EnemyIntentType.DistortNextCard => distortIntentIcon,
                EnemyIntentType.ApplyWeakToPlayer => weakIntentIcon,
                EnemyIntentType.ApplyPoisonToPlayer => poisonIntentIcon,
                EnemyIntentType.HealSelf => healIntentIcon,
                EnemyIntentType.CleanseSelf => cleanseIntentIcon,
                _ => null
            };
        }

        private string GetIntentTitle(EnemyIntentType intentType)
        {
            return intentType switch
            {
                EnemyIntentType.Attack => attackTitle,
                EnemyIntentType.Block => blockTitle,
                EnemyIntentType.Buff => buffTitle,
                EnemyIntentType.DistortNextCard => distortTitle,
                EnemyIntentType.ApplyWeakToPlayer => weakTitle,
                EnemyIntentType.ApplyPoisonToPlayer => poisonTitle,
                EnemyIntentType.HealSelf => healTitle,
                EnemyIntentType.CleanseSelf => cleanseTitle,
                _ => string.Empty
            };
        }

        private string GetIntentDescription(EnemyIntentType intentType, int value)
        {
            return intentType switch
            {
                EnemyIntentType.Attack => string.Format(attackDescriptionFormat, value),
                EnemyIntentType.Block => string.Format(blockDescriptionFormat, value),
                EnemyIntentType.Buff => buffDescription,
                EnemyIntentType.DistortNextCard => distortDescription,
                EnemyIntentType.ApplyWeakToPlayer => string.Format(weakDescriptionFormat, value),
                EnemyIntentType.ApplyPoisonToPlayer => string.Format(poisonDescriptionFormat, value),
                EnemyIntentType.HealSelf => string.Format(healDescriptionFormat, value),
                EnemyIntentType.CleanseSelf => cleanseDescription,
                _ => string.Empty
            };
        }
    }
}