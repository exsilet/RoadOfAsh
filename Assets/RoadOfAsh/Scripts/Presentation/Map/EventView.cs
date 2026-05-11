using System;
using RoadOfAsh.Scripts.Domain.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoadOfAsh.Scripts.Presentation.Map
{
    public class EventView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;

        [SerializeField] private Button firstChoiceButton;
        [SerializeField] private TMP_Text firstChoiceTitleText;
        [SerializeField] private TMP_Text firstChoiceDescriptionText;

        [SerializeField] private Button secondChoiceButton;
        [SerializeField] private TMP_Text secondChoiceTitleText;
        [SerializeField] private TMP_Text secondChoiceDescriptionText;

        private EventChoiceData[] _choices;

        public event Action<EventChoiceData> ChoiceClicked;

        private void Awake()
        {
            Hide();

            if (firstChoiceButton != null)
                firstChoiceButton.onClick.AddListener(() => SelectChoice(0));

            if (secondChoiceButton != null)
                secondChoiceButton.onClick.AddListener(() => SelectChoice(1));
        }

        private void OnDestroy()
        {
            if (firstChoiceButton != null)
                firstChoiceButton.onClick.RemoveAllListeners();

            if (secondChoiceButton != null)
                secondChoiceButton.onClick.RemoveAllListeners();
        }

        public void Show(EventSO eventData)
        {
            if (eventData == null)
                return;

            _choices = eventData.Choices;

            if (panel != null)
                panel.SetActive(true);

            if (titleText != null)
                titleText.text = eventData.Title;

            if (descriptionText != null)
                descriptionText.text = eventData.Description;

            SetupChoice(0, firstChoiceButton, firstChoiceTitleText, firstChoiceDescriptionText);
            SetupChoice(1, secondChoiceButton, secondChoiceTitleText, secondChoiceDescriptionText);
        }

        public void Hide()
        {
            if (panel != null)
                panel.SetActive(false);
        }

        private void SetupChoice(int index, Button button, TMP_Text titleText, TMP_Text descriptionText)
        {
            bool hasChoice = _choices != null && index < _choices.Length && _choices[index] != null;

            if (button != null)
                button.gameObject.SetActive(hasChoice);

            if (!hasChoice)
                return;

            EventChoiceData choice = _choices[index];

            if (titleText != null)
                titleText.text = choice.Title;

            if (descriptionText != null)
                descriptionText.text = choice.Description;
        }

        private void SelectChoice(int index)
        {
            if (_choices == null || index >= _choices.Length)
                return;

            ChoiceClicked?.Invoke(_choices[index]);
        }
    }
}