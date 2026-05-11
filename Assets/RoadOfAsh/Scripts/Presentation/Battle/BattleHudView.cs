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
        [SerializeField] private TMP_Text enemyIntentText;
        [SerializeField] private TMP_Text enemyNameText;

        [Header("Text Formats")]
        [SerializeField] private string enemyHpFormat = "HP: {0}/{1}";
        [SerializeField] private string attackIntentFormat = "Намерение: атака {0}";
        [SerializeField] private string blockIntentFormat = "Намерение: защита {0}";
        [SerializeField] private string buffIntentText = "Намерение: усиление";
        [SerializeField] private string distortIntentText = "Намерение: ведьмовство";
        [SerializeField] private string applyWeakIntentFormat = "Намерение: слабость {0}";
        [SerializeField] private string applyPoisonIntentFormat = "Намерение: яд {0}";
        [SerializeField] private string healSelfIntentFormat = "Намерение: лечение {0}";
        [SerializeField] private string cleanseSelfIntentText = "Намерение: очищение";
        [SerializeField] private string unknownIntentText = "Намерение: неизвестно";

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

            if (enemyIntentText != null)
                enemyIntentText.text = BuildEnemyIntentText(enemy);
        }

        private string BuildEnemyIntentText(EnemyState enemy)
        {
            return enemy.IntentType switch
            {
                EnemyIntentType.Attack => string.Format(attackIntentFormat, enemy.IntentValue),
                EnemyIntentType.Block => string.Format(blockIntentFormat, enemy.IntentValue),
                EnemyIntentType.Buff => buffIntentText,
                EnemyIntentType.DistortNextCard => distortIntentText,
                EnemyIntentType.ApplyWeakToPlayer => string.Format(applyWeakIntentFormat, enemy.IntentValue),
                EnemyIntentType.ApplyPoisonToPlayer => string.Format(applyPoisonIntentFormat, enemy.IntentValue),
                EnemyIntentType.HealSelf => string.Format(healSelfIntentFormat, enemy.IntentValue),
                EnemyIntentType.CleanseSelf => cleanseSelfIntentText,
                _ => unknownIntentText
            };
        }
    }
}