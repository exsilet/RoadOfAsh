using DG.Tweening;
using TMPro;
using UnityEngine;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class BattleEffectsView : MonoBehaviour
    {
        [SerializeField] private TMP_Text floatingTextPrefab;
        [SerializeField] private Transform playerEffectRoot;
        [SerializeField] private Transform enemyEffectRoot;

        public void ShowPlayerDamage(int value)
        {
            ShowText(playerEffectRoot, $"{value}");
        }

        public void ShowEnemyDamage(int value)
        {
            ShowText(enemyEffectRoot, $"{value}");
        }

        public void ShowPlayerBlock(int value)
        {
            ShowText(playerEffectRoot, $"+{value} BLOCK");
        }

        public void ShowEnemyPoison(int value)
        {
            ShowText(enemyEffectRoot, $"-{value} POISON");
        }

        private void ShowText(Transform root, string text)
        {
            if (root == null || floatingTextPrefab == null)
                return;

            TMP_Text instance = Instantiate(floatingTextPrefab, root);
            instance.text = text;

            RectTransform rect = instance.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector2.zero;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(rect.DOAnchorPosY(80f, 0.6f));
            sequence.Join(instance.DOFade(0f, 0.6f));
            sequence.OnComplete(() => Destroy(instance.gameObject));
        }
    }
}