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

        [Header("Buttons")]
        [SerializeField] private Button continueButton;
        [SerializeField] private Button restartRunButton;

        [Header("Timing")]
        [SerializeField] private float resultDelay = 0.8f;

        private Coroutine _showRoutine;

        public event Action ContinueClicked;
        public event Action RestartRunClicked;

        private void Awake()
        {
            HideAll();

            if (continueButton != null)
            {
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(() => ContinueClicked?.Invoke());
            }

            if (restartRunButton != null)
            {
                restartRunButton.onClick.RemoveAllListeners();
                restartRunButton.onClick.AddListener(() => RestartRunClicked?.Invoke());
            }
        }

        private void OnDestroy()
        {
            if (continueButton != null)
                continueButton.onClick.RemoveAllListeners();

            if (restartRunButton != null)
                restartRunButton.onClick.RemoveAllListeners();
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
            _showRoutine = StartCoroutine(ShowBattleResultRoutine(playerWon));
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

            _showRoutine = null;
        }

        private void StopShowRoutine()
        {
            if (_showRoutine == null)
                return;

            StopCoroutine(_showRoutine);
            _showRoutine = null;
        }
    }
}