using System.Collections;
using RoadOfAsh.Scripts.Domain;
using RoadOfAsh.Scripts.Domain.Battle;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Distortion;
using RoadOfAsh.Scripts.Presentation.Battle;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace RoadOfAsh.Scripts.Presentation.Tutorial
{
    public class TutorialBattleFlow : MonoBehaviour
    {
        [SerializeField] private TutorialPanelView tutorialPanel;
        [SerializeField] private RectTransform energyTarget;
        [SerializeField] private Button endTurnButton;
        
        [Header("Highlight")]
        [SerializeField] private TutorialHighlightView highlightView;
        [SerializeField] private RectTransform handTarget;
        [SerializeField] private RectTransform enemyIntentTarget;
        [SerializeField] private RectTransform endTurnButtonTarget;
        [SerializeField] private RectTransform distortionStatusTarget;
        [SerializeField] private RectTransform playerBlockTarget;
        [SerializeField] private RectTransform understandingTarget;
        
        [Header("Card Highlight")]
        [SerializeField] private HandView handView;
        
        [Header("Understanding")]
        [SerializeField] private string understandingTitle = "Понимание сказки";
        [SerializeField] private string understandingDescription = "Понимание растёт после побед. Чем выше понимание, тем реже случайные искажения. Но враги всё ещё могут исказить карту намеренно.";
        
        [Header("Block Explanation")]
        [SerializeField] private string blockInfoTitle = "Блок";
        [SerializeField] private string blockInfoDescription = "Блок снижает входящий урон. Если враг атакует сильнее, остаток урона пройдёт по здоровью.";
        
        [Header("Step 0")]
        [SerializeField] private string energyTitle = "Энергия";
        [SerializeField] private string energyDescription = "Карты тратят энергию. В начале хода у тебя 3 энергии. Стоимость карты указана на самой карте.";

        [Header("Step 1")]
        [SerializeField] private string attackTitle = "Атака";
        [SerializeField] private string attackDescription = "Сыграй карту атаки, чтобы нанести урон врагу.";

        [Header("Step 2")]
        [SerializeField] private string intentTitle = "Намерение врага";
        [SerializeField] private string intentDescription = "Эта иконка показывает, что враг сделает в свой ход. Нажми на неё, чтобы увидеть описание.";
        
        [Header("Step 3")]
        [SerializeField] private string blockTitle = "Блок";
        [SerializeField] private string blockDescription = "Сыграй карту блока, чтобы защититься от атаки врага.";

        [Header("Step 4")]
        [SerializeField] private string endTurnTitle = "Конец хода";
        [SerializeField] private string endTurnDescription = "Когда закончишь разыгрывать карты, нажми кнопку конца хода.";

        [Header("Step 5")]
        [SerializeField] private string distortionTitle = "Искажение";
        [SerializeField] private string distortionDescription = "Если враг показывает эту иконку, он исказит следующую карту после своего хода. Следи за намерением врага.";

        private IBattleService _battleService;
        private IDistortionService _distortionService;
        private RunState _runState;
        private TutorialStep _currentStep;
        private bool _active;

        private enum TutorialStep
        {
            None,
            ExplainEnergy,
            PlayAttack,
            ExplainIntent,
            ExplainBlock,
            PlayBlock,
            ExplainEndTurn,
            ExplainDistortion,
            ExplainUnderstanding,
            Done
        }

        [Inject]
        public void Construct(IBattleService battleService, RunState runState, IDistortionService distortionService)
        {
            _battleService = battleService;
            _runState = runState;
            _distortionService = distortionService;
        }

        private void Start()
        {
            if (_runState == null)
            {
                Debug.LogError("TutorialBattleFlow: RunState is null.");
                HideTutorial();
                return;
            }

            if (_runState.IntroBattleCompleted)
            {
                HideTutorial();
                return;
            }

            _active = true;
            _distortionService?.SetRandomDistortionEnabled(false);

            if (_battleService != null)
            {
                _battleService.OnCardPlayed += OnCardPlayed;
                _battleService.OnPlayerTurnEnded += OnPlayerTurnEnded;
            }
            else
            {
                Debug.LogError("TutorialBattleFlow: BattleService is null.");
            }

            SetStep(TutorialStep.ExplainEnergy);
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
        
        public bool CanPlayCard(CardSO card)
        {
            if (!_active)
                return true;

            if (card == null)
                return false;

            return _currentStep switch
            {
                TutorialStep.PlayAttack => CardHasEffect(card, EffectType.Damage),
                TutorialStep.PlayBlock => CardHasEffect(card, EffectType.Block),

                TutorialStep.ExplainEnergy => false,
                TutorialStep.ExplainIntent => false,
                TutorialStep.ExplainBlock => false,
                TutorialStep.ExplainEndTurn => false,
                TutorialStep.ExplainDistortion => false,
                TutorialStep.ExplainUnderstanding => false,

                TutorialStep.None => true,
                TutorialStep.Done => true,
                _ => true
            };
        }
        
        private bool CardHasEffect(CardSO card, EffectType effectType)
        {
            if (card.Effects == null)
                return false;

            foreach (CardEffect effect in card.Effects)
            {
                if (effect.Type == effectType)
                    return true;
            }

            return false;
        }
        
        private void OnPlayerTurnEnded()
        {
            if (!_active)
                return;

            if (_currentStep == TutorialStep.ExplainEndTurn)
                SetStep(TutorialStep.ExplainDistortion);
        }
        
        public bool CanEndTurn()
        {
            if (!_active)
                return true;

            return _currentStep == TutorialStep.ExplainEndTurn ||
                   _currentStep == TutorialStep.Done;
        }

        private void SetStep(TutorialStep step)
        {
            _currentStep = step;
            
            SetEndTurnButtonInteractable(false);

            switch (step)
            {
                case TutorialStep.ExplainEnergy:
                    ShowMessage(energyTitle, energyDescription, true, () => SetStep(TutorialStep.PlayAttack));
                    ShowHighlight(energyTarget);
                    break;
                case TutorialStep.PlayAttack:
                    ShowMessage(attackTitle, attackDescription, false);
                    ShowCardHighlight(EffectType.Damage);
                    break;
                case TutorialStep.ExplainIntent:
                    ShowMessage(intentTitle, intentDescription, true, () => SetStep(TutorialStep.ExplainBlock));
                    ShowHighlight(enemyIntentTarget);
                    break;
                case TutorialStep.ExplainBlock:
                    ShowMessage(blockInfoTitle, blockInfoDescription, true, () => SetStep(TutorialStep.PlayBlock));
                    ShowHighlight(playerBlockTarget);
                    break;
                case TutorialStep.PlayBlock:
                    ShowMessage(blockTitle, blockDescription, false);
                    ShowCardHighlight(EffectType.Block);
                    break;
                case TutorialStep.ExplainEndTurn:
                    ShowMessage(endTurnTitle, endTurnDescription, false);
                    ShowHighlight(endTurnButtonTarget);
                    SetEndTurnButtonInteractable(true);
                    break;
                case TutorialStep.ExplainDistortion:
                    ShowMessage(distortionTitle, distortionDescription, true, () => SetStep(TutorialStep.ExplainUnderstanding));
                    ShowHighlight(enemyIntentTarget);
                    break;
                case TutorialStep.ExplainUnderstanding:
                    ShowMessage(understandingTitle, understandingDescription, true, CompleteTutorialHints);
                    ShowHighlight(understandingTarget);
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
        
        private void ShowCardHighlight(EffectType effectType)
        {
            StartCoroutine(ShowCardHighlightRoutine(effectType));
        }

        private IEnumerator ShowCardHighlightRoutine(EffectType effectType)
        {
            yield return null;

            RectTransform target = null;

            if (handView != null)
                target = handView.FindFirstCardWithEffect(effectType);

            if (target == null)
                target = handTarget;

            ShowHighlight(target);
        }
    }
}