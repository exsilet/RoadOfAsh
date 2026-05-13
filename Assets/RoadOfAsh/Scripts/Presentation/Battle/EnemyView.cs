using RoadOfAsh.Scripts.Domain.Battle;
using TMPro;
using UnityEngine;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class EnemyView : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text hpText;

        public void Setup(EnemyState enemy)
        {
            if (enemy == null)
                return;

            if (nameText != null)
                nameText.text = enemy.Name;

            if (hpText != null)
                hpText.text = $"HP: {enemy.HP}/{enemy.MaxHP}";
        }
    }
}