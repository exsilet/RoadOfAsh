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
            nameText.text = enemy.Name;
            hpText.text = $"HP: {enemy.HP}/{enemy.MaxHP}";
        }
    }
}