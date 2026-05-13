using RoadOfAsh.Scripts.Domain;
using RoadOfAsh.Scripts.Domain.Battle;
using RoadOfAsh.Scripts.Domain.Cards;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace RoadOfAsh.Scripts.Presentation.Tutorial
{
    public class TutorialBattleFlow : MonoBehaviour
    {
        [SerializeField] private TutorialPanelView tutorialPanel;
        [SerializeField] private Button endTurnButton;
        
        [Header("Highlight")]
        [SerializeField] private TutorialHighlightView highlightView;
        [SerializeField] private RectTransform handTarget;
        [SerializeField] private RectTransform enemyIntentTarget;
        [SerializeField] private RectTransform endTurnButtonTarget;
        [SerializeField] private RectTransform distortionStatusTarget;

        [Header("Step 1")]
        [SerializeField] private string attackTitle = "Атака";
        [SerializeField] private string attackDescription = "Сыграй карту атаки, чтобы нанести урон врагу.";

        [Header("Step 2")]
        [SerializeField] private string intentTitle = "Намерение врага";
        [SerializeField] private string intentDescription = "Иконка рядом с врагом показывает, что он сделает в свой ход. Нажми на неё, чтобы увидеть описание.";

        [Header("Step 3")]
        [SerializeField] private string blockTitle = "Блок";
        [SerializeField] private string blockDescription = "Сыграй карту блока, чтобы защититься от атаки врага.";

        [Header("Step 4")]
        [SerializeField] private string endTurnTitle = "Конец хода";
        [SerializeField] private string endTurnDescription = "Когда закончишь разыгрывать карты, нажми кнопку конца хода.";

        [Header("Step 5")]
        [SerializeField] private string distortionTitle = "Искажение";
        [SerializeField] private string distortionDescription = "Некоторые враги могут исказить следующую карту. Следи за иконкой искажения рядом со статусами.";

        private IBattleService _battleService;
        private RunState _runState;
        private TutorialStep _currentStep;
        private bool _active;

        private enum TutorialStep
        {
            None,
            PlayAttack,
            ExplainIntent,
            PlayBlock,
            ExplainEndTurn,
            ExplainDistortion,
            Done
        }

        [Inject]
        public void Construct(IBattleService battleService, RunState runState)
        {
            _battleService = battleService;
            _runState = runState;
        }

        private void Start()
        {
            Debug.Log("TutorialBattleFlow START");

            if (_runState == null)
            {
                Debug.LogError("TutorialBattleFlow: RunState is null");
                return;
            }

            Debug.Log($"IntroBattleCompleted = {_runState.IntroBattleCompleted}");

            if (_runState.IntroBattleCompleted)
            {
                HideTutorial();
                return;
            }

            _active = true;

            if (_battleService != null)
            {
                _battleService.OnCardPlayed += OnCardPlayed;
                _battleService.OnPlayerTurnEnded += OnPlayerTurnEnded;
            }
            else
            {
                Debug.LogError("TutorialBattleFlow: BattleService is null");
            }

            SetStep(TutorialStep.PlayAttack);
        }

        private void OnDestroy()
        {
            if (_battleService != null)
            {
                _battleService.OnCardPlayed -= OnCardPlayed;
                _battleService.OnPlayerTurnEnded -= OnPlayerTurnEnded;
            }
        }

        private void OnCardPlayed(CardSO card, PlayedCardResult result)
        {
            if (!_active || result == null)
                return;

            switch (_currentStep)
            {
                case TutorialStep.PlayAttack:
                    if (HasEffect(result, EffectType.Damage))
                        SetStep(TutorialStep.ExplainIntent);
                    break;

                case TutorialStep.PlayBlock:
                    if (HasEffect(result, EffectType.Block))
                        SetStep(TutorialStep.ExplainEndTurn);
                    break;
            }
        }
        
        private void OnPlayerTurnEnded()
        {
            if (!_active)
                return;

            if (_currentStep == TutorialStep.ExplainEndTurn)
                SetStep(TutorialStep.ExplainDistortion);
        }

        private void SetStep(TutorialStep step)
        {
            _currentStep = step;
            
            SetEndTurnButtonInteractable(false);

            switch (step)
            {
                case TutorialStep.PlayAttack:
                    ShowMessage(attackTitle, attackDescription, false);
                    ShowHighlight(handTarget);
                    break;
                case TutorialStep.ExplainIntent:
                    ShowMessage(intentTitle, intentDescription, true, () => SetStep(TutorialStep.PlayBlock));
                    ShowHighlight(enemyIntentTarget);
                    break;
                case TutorialStep.PlayBlock:
                    ShowMessage(blockTitle, blockDescription, false);
                    ShowHighlight(handTarget);
                    break;
                case TutorialStep.ExplainEndTurn:
                    ShowMessage(endTurnTitle, endTurnDescription, false);
                    ShowHighlight(endTurnButtonTarget);
                    SetEndTurnButtonInteractable(true);
                    break;
                case TutorialStep.ExplainDistortion:
                    ShowMessage(distortionTitle, distortionDescription, true, CompleteTutorialHints);
                    ShowHighlight(distortionStatusTarget);
                    break;
                case TutorialStep.Done:
                    SetEndTurnButtonInteractable(true);
                    HideTutorial();
                    break;
            }
        }

        private bool HasEffect(PlayedCardResult result, EffectType effectType)
        {
            if (result.FinalEffects == null)
                return false;

            foreach (CardEffect effect in result.FinalEffects)
            {
                if (effect.Type == effectType)
                    return true;
            }

            return false;
        }

        private void ShowMessage(string title, string description, bool showNextButton, System.Action nextClicked = null)
        {
            if (tutorialPanel == null)
                return;

            tutorialPanel.Show(title, description, showNextButton, nextClicked);
        }

        private void CompleteTutorialHints()
        {
            SetStep(TutorialStep.Done);
        }

        private void HideTutorial()
        {
            _active = false;
            
            SetEndTurnButtonInteractable(true);

            if (tutorialPanel != null)
                tutorialPanel.Hide();
            
            HideHighlight();
        }

        private void ShowHighlight(RectTransform target)
        {
            if (highlightView == null)
                return;

            highlightView.Show(target);
        }

        private void HideHighlight()
        {
            if (highlightView != null)
                highlightView.Hide();
        }
        
        private void SetEndTurnButtonInteractable(bool interactable)
        {
            if (endTurnButton != null)
                endTurnButton.interactable = interactable;
        }
    }
}