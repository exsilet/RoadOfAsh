using System;
using System.Collections;
using DG.Tweening;
using RoadOfAsh.Scripts.Domain.Battle;
using RoadOfAsh.Scripts.Presentation.Tutorial;
using UnityEngine;
using UnityEngine.UI;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class BattleTurnFlow : MonoBehaviour
    {
        [SerializeField] private Button endTurnButton;
        [SerializeField] private HandView handView;
        [SerializeField] private CardPlayAnimator cardPlayAnimator;
        [SerializeField] private TutorialBattleFlow tutorialBattleFlow;

        private IBattleService _battleService;
        private Func<bool> _isBusy;

        private bool _isEndingTurn;

        public bool IsEndingTurn => _isEndingTurn;

        public event Action TurnEnded;

        public void Initialize(IBattleService battleService, Func<bool> isBusy)
        {
            _battleService = battleService;
            _isBusy = isBusy;

            if (endTurnButton != null)
            {
                endTurnButton.onClick.RemoveAllListeners();
                endTurnButton.onClick.AddListener(TryEndTurn);
            }
        }

        public void Dispose()
        {
            if (endTurnButton != null)
                endTurnButton.onClick.RemoveListener(TryEndTurn);
        }

        public void RefreshButton()
        {
            if (endTurnButton == null || _battleService == null)
                return;

            bool canEndTurn =
                !_battleService.IsBattleFinished &&
                !IsBusy() &&
                CanEndTurnByTutorial();

            endTurnButton.interactable = canEndTurn;
        }

        public void TryEndTurn()
        {
            if (_battleService == null)
                return;

            if (IsBusy())
                return;

            if (!CanEndTurnByTutorial())
            {
                ShakeEndTurnButton();
                return;
            }

            StartCoroutine(EndTurnRoutine());
        }

        private IEnumerator EndTurnRoutine()
        {
            _isEndingTurn = true;
            RefreshButton();

            if (cardPlayAnimator != null && handView != null)
                yield return cardPlayAnimator.DiscardHand(handView.Root);

            _battleService.EndPlayerTurn();

            _isEndingTurn = false;

            TurnEnded?.Invoke();
            RefreshButton();
        }

        private bool CanEndTurnByTutorial()
        {
            return tutorialBattleFlow == null || tutorialBattleFlow.CanEndTurn();
        }

        private bool IsBusy()
        {
            return _isEndingTurn || (_isBusy != null && _isBusy.Invoke());
        }

        private void ShakeEndTurnButton()
        {
            if (endTurnButton == null)
                return;

            RectTransform rect = endTurnButton.GetComponent<RectTransform>();

            if (rect == null)
                return;

            rect.DOKill();
            rect.DOShakeAnchorPos(0.25f, new Vector2(14f, 0f), 10, 90f);
        }
    }
}