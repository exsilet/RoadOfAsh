using System;
using System.Collections;
using DG.Tweening;
using RoadOfAsh.Scripts.Domain.Battle;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Players;
using RoadOfAsh.Scripts.Presentation.Tutorial;
using UnityEngine;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class BattleCardPlayFlow : MonoBehaviour
    {
        [SerializeField] private CardPlayAnimator cardPlayAnimator;
        [SerializeField] private CardResultView cardResultView;
        [SerializeField] private TutorialBattleFlow tutorialBattleFlow;

        private IBattleService _battleService;
        private PlayerState _playerState;

        private bool _isCardAnimating;

        public bool IsCardAnimating => _isCardAnimating;

        public event Action CardPlayFinished;

        public void Initialize(IBattleService battleService, PlayerState playerState)
        {
            _battleService = battleService;
            _playerState = playerState;
        }

        public bool TryHandleCardClicked(CardView cardView, CardSO card)
        {
            if (_isCardAnimating)
                return true;

            if (_battleService == null || _battleService.IsBattleFinished)
                return true;

            if (cardView == null || card == null)
                return true;

            if (tutorialBattleFlow != null && !tutorialBattleFlow.CanPlayCard(card))
            {
                ShowWrongTutorialCard(cardView);
                return true;
            }

            if (_playerState == null || _playerState.Energy < card.Cost)
            {
                ShowNotEnoughEnergy(cardView);
                return true;
            }

            StartCoroutine(PlayCardWithDiscardAnimation(cardView, card));
            return true;
        }

        private void ShowNotEnoughEnergy(CardView cardView)
        {
            ShakeCard(cardView, 18f, 12);

            if (cardResultView != null)
                cardResultView.ShowNotEnoughEnergy();
        }

        private void ShowWrongTutorialCard(CardView cardView)
        {
            ShakeCard(cardView, 14f, 10);
        }

        private void ShakeCard(CardView cardView, float strength, int vibrato)
        {
            RectTransform rect = cardView.GetComponent<RectTransform>();

            if (rect == null)
                return;

            rect.DOKill();
            rect.DOShakeAnchorPos(0.25f, new Vector2(strength, 0f), vibrato, 90f);
        }

        private IEnumerator PlayCardWithDiscardAnimation(CardView cardView, CardSO card)
        {
            _isCardAnimating = true;

            if (cardPlayAnimator != null)
                yield return cardPlayAnimator.MoveToPlay(cardView);

            bool played = _battleService.TryPlayCard(card);

            if (played && cardPlayAnimator != null)
                yield return cardPlayAnimator.MoveToDiscard(cardView);

            _isCardAnimating = false;
            CardPlayFinished?.Invoke();
        }
    }
}