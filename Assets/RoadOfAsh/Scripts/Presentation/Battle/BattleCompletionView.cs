using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class BattleCompletionView : MonoBehaviour
    {
        [SerializeField] private BattleResultView battleResultView;
        [SerializeField] private GameObject chapterCompletePanel;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button restartRunButton;

        [SerializeField] private float resultDelay = 0.8f;

        private Coroutine _showResultRoutine;

        public event Action ContinueClicked;
        public event Action RestartRunClicked;

        private void Awake()
        {
            HideAll();

            if (continueButton != null)
                continueButton.onClick.AddListener(OnContinueClicked);

            if (restartRunButton != null)
                restartRunButton.onClick.AddListener(OnRestartRunClicked);
        }

        private void OnDestroy()
        {
            if (continueButton != null)
                continueButton.onClick.RemoveListener(OnContinueClicked);

            if (restartRunButton != null)
                restartRunButton.onClick.RemoveListener(OnRestartRunClicked);
        }

        public void HideAll()
        {
            if (battleResultView != null)
                battleResultView.Hide();

            if (chapterCompletePanel != null)
                chapterCompletePanel.SetActive(false);
        }

        public void SetContinueInteractable(bool value)
        {
            if (continueButton != null)
                continueButton.interactable = value;
        }

        public void ShowBattleResult(bool playerWon)
        {
            StopShowRoutine();
            _showResultRoutine = StartCoroutine(ShowBattleResultRoutine(playerWon));
        }

        public void ShowChapterComplete()
        {
            if (battleResultView != null)
                battleResultView.Hide();

            if (chapterCompletePanel != null)
                chapterCompletePanel.SetActive(true);
        }

        private IEnumerator ShowBattleResultRoutine(bool playerWon)
        {
            yield return new WaitForSeconds(resultDelay);

            if (battleResultView != null)
                battleResultView.Show(playerWon);

            _showResultRoutine = null;
        }

        private void StopShowRoutine()
        {
            if (_showResultRoutine == null)
                return;

            StopCoroutine(_showResultRoutine);
            _showResultRoutine = null;
        }

        private void OnContinueClicked()
        {
            ContinueClicked?.Invoke();
        }

        private void OnRestartRunClicked()
        {
            RestartRunClicked?.Invoke();
        }
    }
}