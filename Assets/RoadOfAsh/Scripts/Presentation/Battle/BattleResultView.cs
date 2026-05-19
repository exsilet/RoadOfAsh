using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class BattleResultView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text resultText;

        [Header("Buttons")]
        [SerializeField] private Button continueButton;
        [SerializeField] private Button freeReviveButton;
        [SerializeField] private Button rewardReviveButton;
        [SerializeField] private Button restartRunButton;
        [SerializeField] private Button mainMenuButton;

        [Header("Texts")]
        [SerializeField] private string victoryText = "ПОБЕДА";
        [SerializeField] private string defeatText = "ПОРАЖЕНИЕ";

        public event Action ContinueClicked;
        public event Action FreeReviveClicked;
        public event Action RewardReviveClicked;
        public event Action RestartRunClicked;
        public event Action MainMenuClicked;

        private void Awake()
        {
            if (continueButton != null)
                continueButton.onClick.AddListener(() => ContinueClicked?.Invoke());

            if (freeReviveButton != null)
                freeReviveButton.onClick.AddListener(() => FreeReviveClicked?.Invoke());

            if (rewardReviveButton != null)
                rewardReviveButton.onClick.AddListener(() => RewardReviveClicked?.Invoke());

            if (restartRunButton != null)
                restartRunButton.onClick.AddListener(() => RestartRunClicked?.Invoke());

            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(() => MainMenuClicked?.Invoke());

            Hide();
        }

        private void OnDestroy()
        {
            if (continueButton != null)
                continueButton.onClick.RemoveAllListeners();

            if (freeReviveButton != null)
                freeReviveButton.onClick.RemoveAllListeners();

            if (rewardReviveButton != null)
                rewardReviveButton.onClick.RemoveAllListeners();

            if (restartRunButton != null)
                restartRunButton.onClick.RemoveAllListeners();

            if (mainMenuButton != null)
                mainMenuButton.onClick.RemoveAllListeners();
        }

        public void Hide()
        {
            if (panel != null)
                panel.SetActive(false);

            SetButtonVisible(continueButton, false);
            SetButtonVisible(freeReviveButton, false);
            SetButtonVisible(rewardReviveButton, false);
            SetButtonVisible(restartRunButton, false);
            SetButtonVisible(mainMenuButton, false);
        }

        public void ShowVictory()
        {
            ShowBase(victoryText);

            SetButtonVisible(continueButton, true);
            SetButtonVisible(freeReviveButton, false);
            SetButtonVisible(rewardReviveButton, false);
            SetButtonVisible(restartRunButton, false);
            SetButtonVisible(mainMenuButton, false);
        }

        public void ShowFirstDefeat()
        {
            ShowBase(defeatText);

            SetButtonVisible(continueButton, false);
            SetButtonVisible(freeReviveButton, true);
            SetButtonVisible(rewardReviveButton, true);
            SetButtonVisible(restartRunButton, true);
            SetButtonVisible(mainMenuButton, false);
        }

        public void ShowFinalDefeat()
        {
            ShowBase(defeatText);

            SetButtonVisible(continueButton, false);
            SetButtonVisible(freeReviveButton, false);
            SetButtonVisible(rewardReviveButton, false);
            SetButtonVisible(restartRunButton, true);
            SetButtonVisible(mainMenuButton, true);
        }

        public void SetContinueInteractable(bool interactable)
        {
            if (continueButton != null)
                continueButton.interactable = interactable;
        }

        public void SetRewardReviveInteractable(bool interactable)
        {
            if (rewardReviveButton != null)
                rewardReviveButton.interactable = interactable;
        }

        private void ShowBase(string text)
        {
            if (panel != null)
                panel.SetActive(true);

            if (resultText != null)
                resultText.text = text;
        }

        private void SetButtonVisible(Button button, bool visible)
        {
            if (button != null)
                button.gameObject.SetActive(visible);
        }
    }
}