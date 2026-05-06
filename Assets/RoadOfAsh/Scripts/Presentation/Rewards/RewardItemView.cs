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
        [Header("Card Reward")]
        [SerializeField] private Transform cardRoot;
        [SerializeField] private CardView cardPrefab;

        [Header("Text Formats")]
        [SerializeField] private string goldFormat = "+{0} золота";
        [SerializeField] private string healFormat = "+{0} HP";

        private RewardItem _rewardItem;
        private System.Action<RewardItem> _onClicked;

        public void Setup(RewardItem rewardItem, System.Action<RewardItem> onClicked)
        {
            _rewardItem = rewardItem;
            _onClicked = onClicked;

            RefreshView();

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnClicked);
            }
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
                    ShowSimpleReward(string.Format(goldFormat, _rewardItem.Amount), _rewardItem.Icon);
                    break;

                case RewardType.Heal:
                    ShowSimpleReward(string.Format(healFormat, _rewardItem.Amount), _rewardItem.Icon);
                    break;
            }
        }

        private void ShowCardReward()
        {
            if (artImage != null)
                artImage.gameObject.SetActive(false);

            if (titleText != null)
                titleText.gameObject.SetActive(false);

            if (cardPrefab == null || cardRoot == null || _rewardItem.Card == null)
                return;

            CardView cardView = Instantiate(cardPrefab, cardRoot);
            cardView.Setup(_rewardItem.Card, false);

            DisableRaycasts(cardView.gameObject);
        }

        private void ShowSimpleReward(string title, Sprite icon)
        {
            if (artImage != null)
            {
                artImage.gameObject.SetActive(true);
                artImage.sprite = icon;
                artImage.enabled = icon != null;
            }

            if (titleText != null)
            {
                titleText.gameObject.SetActive(true);
                titleText.text = title;
            }
        }

        private void ClearCardRoot()
        {
            if (cardRoot == null)
                return;

            for (int i = cardRoot.childCount - 1; i >= 0; i--)
                Destroy(cardRoot.GetChild(i).gameObject);
        }

        private void DisableRaycasts(GameObject root)
        {
            Graphic[] graphics = root.GetComponentsInChildren<Graphic>(true);

            foreach (Graphic graphic in graphics)
                graphic.raycastTarget = false;
        }

        private void OnClicked()
        {
            _onClicked?.Invoke(_rewardItem);
        }
    }
}