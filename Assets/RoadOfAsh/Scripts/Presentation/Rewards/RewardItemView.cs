using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain.Rewards;
using RoadOfAsh.Scripts.Presentation.Battle;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoadOfAsh.Scripts.Presentation.Rewards
{
    public sealed class RewardItemView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image artImage;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private Transform cardRoot;
        [SerializeField] private CardView cardPrefab;

        private RewardItem _rewardItem;
        private System.Action<RewardItem> _onClicked;

        public void Setup(RewardItem rewardItem, System.Action<RewardItem> onClicked)
        {
            _rewardItem = rewardItem;
            _onClicked = onClicked;

            RefreshView();

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClicked);
        }

        private void RefreshView()
        {
            ClearCardRoot();

            switch (_rewardItem.Type)
            {
                case RewardType.Card:
                    ShowCardReward();
                    break;

                case RewardType.Gold:
                    ShowSimpleReward($"+{_rewardItem.Amount} золота", _rewardItem.Icon);
                    break;

                case RewardType.Heal:
                    ShowSimpleReward($"+{_rewardItem.Amount} HP", _rewardItem.Icon);
                    break;
            }
        }
        
        private void ShowCardReward()
        {
            artImage.gameObject.SetActive(false);
            titleText.gameObject.SetActive(false);

            if (cardPrefab == null || cardRoot == null || _rewardItem.Card == null)
                return;

            CardView cardView = Instantiate(cardPrefab, cardRoot);
            cardView.Setup(_rewardItem.Card, false);

            DisableRaycasts(cardView.gameObject);
        }
        
        private void DisableRaycasts(GameObject root)
        {
            Graphic[] graphics = root.GetComponentsInChildren<Graphic>(true);

            foreach (Graphic graphic in graphics)
                graphic.raycastTarget = false;
        }

        private void ShowSimpleReward(string title, Sprite icon)
        {
            artImage.gameObject.SetActive(true);
            titleText.gameObject.SetActive(true);

            titleText.text = title;
            artImage.sprite = icon;
            artImage.enabled = icon != null;
        }

        private void ClearCardRoot()
        {
            if (cardRoot == null)
                return;

            for (int i = cardRoot.childCount - 1; i >= 0; i--)
                Destroy(cardRoot.GetChild(i).gameObject);
        }

        private void OnClicked()
        {
            _onClicked?.Invoke(_rewardItem);
        }
    }
}