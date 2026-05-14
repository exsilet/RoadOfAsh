using System;
using UnityEngine;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class BattleResultFlow : MonoBehaviour
    {
        [SerializeField] private BattleCompletionView battleCompletionView;

        private bool _finishShown;

        public event Action ContinueClicked;
        public event Action RestartRunClicked;

        public void Initialize()
        {
            _finishShown = false;

            if (battleCompletionView == null)
                return;

            battleCompletionView.HideAll();
            battleCompletionView.ContinueClicked += OnContinueClicked;
            battleCompletionView.RestartRunClicked += OnRestartRunClicked;
        }

        public void Dispose()
        {
            if (battleCompletionView == null)
                return;

            battleCompletionView.ContinueClicked -= OnContinueClicked;
            battleCompletionView.RestartRunClicked -= OnRestartRunClicked;
        }

        public void Refresh(bool isBattleFinished, bool playerWon)
        {
            if (!isBattleFinished)
            {
                _finishShown = false;

                if (battleCompletionView != null)
                    battleCompletionView.HideAll();

                return;
            }

            if (_finishShown)
                return;

            _finishShown = true;

            if (battleCompletionView != null)
                battleCompletionView.ShowBattleResult(playerWon);
        }

        public void Hide()
        {
            if (battleCompletionView != null)
                battleCompletionView.HideAll();
        }

        public void ShowChapterComplete()
        {
            if (battleCompletionView != null)
                battleCompletionView.ShowChapterComplete();
        }

        public void SetContinueInteractable(bool interactable)
        {
            if (battleCompletionView != null)
                battleCompletionView.SetContinueInteractable(interactable);
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