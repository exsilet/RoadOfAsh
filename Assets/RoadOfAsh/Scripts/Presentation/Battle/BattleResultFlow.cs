using System;
using UnityEngine;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class BattleResultFlow : MonoBehaviour
    {
        [SerializeField] private BattleResultView battleResultView;
        [SerializeField] private BattleCompletionView battleCompletionView;

        private bool _finishShown;
        private bool _reviveOptionsAvailable = true;

        public event Action ContinueClicked;
        public event Action FreeReviveClicked;
        public event Action RewardReviveClicked;
        public event Action RestartRunClicked;
        public event Action MainMenuClicked;
        public event Action ChapterContinueClicked;

        public void Initialize()
        {
            _finishShown = false;
            _reviveOptionsAvailable = true;

            if (battleResultView != null)
            {
                battleResultView.Hide();

                battleResultView.ContinueClicked += OnContinueClicked;
                battleResultView.FreeReviveClicked += OnFreeReviveClicked;
                battleResultView.RewardReviveClicked += OnRewardReviveClicked;
                battleResultView.RestartRunClicked += OnRestartRunClicked;
                battleResultView.MainMenuClicked += OnMainMenuClicked;
            }

            if (battleCompletionView != null)
            {
                battleCompletionView.Hide();
                battleCompletionView.ContinueClicked += OnChapterContinueClicked;
            }
        }

        public void Dispose()
        {
            if (battleResultView != null)
            {
                battleResultView.ContinueClicked -= OnContinueClicked;
                battleResultView.FreeReviveClicked -= OnFreeReviveClicked;
                battleResultView.RewardReviveClicked -= OnRewardReviveClicked;
                battleResultView.RestartRunClicked -= OnRestartRunClicked;
                battleResultView.MainMenuClicked -= OnMainMenuClicked;
            }

            if (battleCompletionView != null)
                battleCompletionView.ContinueClicked -= OnChapterContinueClicked;
        }

        public void Refresh(bool isBattleFinished, bool playerWon)
        {
            if (!isBattleFinished)
            {
                _finishShown = false;
                Hide();
                return;
            }

            if (_finishShown)
                return;

            _finishShown = true;

            if (playerWon)
            {
                if (battleResultView != null)
                    battleResultView.ShowVictory();

                return;
            }

            if (battleResultView == null)
                return;

            if (_reviveOptionsAvailable)
                battleResultView.ShowFirstDefeat();
            else
                battleResultView.ShowFinalDefeat();
        }

        public void Hide()
        {
            if (battleResultView != null)
                battleResultView.Hide();

            if (battleCompletionView != null)
                battleCompletionView.Hide();
        }

        public void ShowChapterComplete()
        {
            if (battleResultView != null)
                battleResultView.Hide();

            if (battleCompletionView != null)
                battleCompletionView.Show();
        }

        public void SetDefeatReviveOptionsAvailable(bool available)
        {
            _reviveOptionsAvailable = available;
        }

        public void SetContinueInteractable(bool interactable)
        {
            if (battleResultView != null)
                battleResultView.SetContinueInteractable(interactable);

            if (battleCompletionView != null)
                battleCompletionView.SetContinueInteractable(interactable);
        }

        public void SetRewardReviveInteractable(bool interactable)
        {
            if (battleResultView != null)
                battleResultView.SetRewardReviveInteractable(interactable);
        }

        private void OnContinueClicked()
        {
            ContinueClicked?.Invoke();
        }

        private void OnFreeReviveClicked()
        {
            FreeReviveClicked?.Invoke();
        }

        private void OnRewardReviveClicked()
        {
            RewardReviveClicked?.Invoke();
        }

        private void OnRestartRunClicked()
        {
            RestartRunClicked?.Invoke();
        }

        private void OnMainMenuClicked()
        {
            MainMenuClicked?.Invoke();
        }

        private void OnChapterContinueClicked()
        {
            ChapterContinueClicked?.Invoke();
        }
    }
}