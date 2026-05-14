using UnityEngine;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class BattleEffectsView : MonoBehaviour
    {
        [Header("Popup")]
        [SerializeField] private BattleEffectPopup popupPrefab;
        [SerializeField] private Transform playerEffectRoot;
        [SerializeField] private Transform enemyEffectRoot;

        [Header("Icons")]
        [SerializeField] private Sprite damageIcon;
        [SerializeField] private Sprite blockIcon;
        [SerializeField] private Sprite poisonIcon;
        [SerializeField] private Sprite weakIcon;
        [SerializeField] private Sprite healIcon;
        [SerializeField] private Sprite cleanseIcon;

        [Header("VFX Prefabs")]
        [SerializeField] private GameObject playerDamageVfxPrefab;
        [SerializeField] private GameObject enemyDamageVfxPrefab;
        [SerializeField] private GameObject playerBlockVfxPrefab;
        [SerializeField] private GameObject enemyPoisonVfxPrefab;
        [SerializeField] private GameObject playerPoisonVfxPrefab;
        [SerializeField] private GameObject playerWeakVfxPrefab;
        [SerializeField] private GameObject enemyHealVfxPrefab;
        [SerializeField] private GameObject enemyCleanseVfxPrefab;
        
        [Header("Relic VFX")]
        [SerializeField] private Sprite relicBlockDistortionIcon;
        [SerializeField] private GameObject relicBlockDistortionVfxPrefab;

        [Header("Animation")]
        [SerializeField] private float popupMoveY = 80f;
        [SerializeField] private float popupDuration = 0.6f;
        [SerializeField] private float vfxLifetime = 1.2f;

        public void ShowPlayerDamage(int value)
        {
            ShowPopup(playerEffectRoot, damageIcon, value);
            PlayVfx(playerEffectRoot, playerDamageVfxPrefab);
        }

        public void ShowEnemyDamage(int value)
        {
            ShowPopup(enemyEffectRoot, damageIcon, value);
            PlayVfx(enemyEffectRoot, enemyDamageVfxPrefab);
        }

        public void ShowPlayerBlock(int value)
        {
            ShowPopup(playerEffectRoot, blockIcon, value);
            PlayVfx(playerEffectRoot, playerBlockVfxPrefab);
        }

        public void ShowEnemyPoison(int value)
        {
            ShowPopup(enemyEffectRoot, poisonIcon, value);
            PlayVfx(enemyEffectRoot, enemyPoisonVfxPrefab);
        }

        public void ShowPlayerPoison(int value)
        {
            ShowPopup(playerEffectRoot, poisonIcon, value);
            PlayVfx(playerEffectRoot, playerPoisonVfxPrefab);
        }

        public void ShowPlayerWeak(int value)
        {
            ShowPopup(playerEffectRoot, weakIcon, value);
            PlayVfx(playerEffectRoot, playerWeakVfxPrefab);
        }

        public void ShowEnemyHeal(int value)
        {
            ShowPopup(enemyEffectRoot, healIcon, value);
            PlayVfx(enemyEffectRoot, enemyHealVfxPrefab);
        }

        public void ShowEnemyCleanse()
        {
            ShowPopup(enemyEffectRoot, cleanseIcon, null);
            PlayVfx(enemyEffectRoot, enemyCleanseVfxPrefab);
        }
        
        public void ShowRelicBlockedDistortion()
        {
            ShowPopup(playerEffectRoot, relicBlockDistortionIcon, null);
            PlayVfx(playerEffectRoot, relicBlockDistortionVfxPrefab);
        }

        private void ShowPopup(Transform root, Sprite icon, int? value)
        {
            if (root == null || popupPrefab == null)
                return;

            BattleEffectPopup popup = Instantiate(popupPrefab, root);
            popup.Setup(icon, value);
            popup.Play(popupMoveY, popupDuration);
        }

        private void PlayVfx(Transform root, GameObject prefab)
        {
            if (root == null || prefab == null)
                return;

            GameObject instance = Instantiate(prefab, root);
            instance.transform.localPosition = Vector3.zero;
            Destroy(instance, vfxLifetime);
        }
    }
}