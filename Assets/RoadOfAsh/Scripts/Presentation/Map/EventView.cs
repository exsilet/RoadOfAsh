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
        [SerializeField] private Transform choicesRoot;
        [SerializeField] private EventChoiceView choicePrefab;

        public event Action<EventChoiceData> ChoiceClicked;

        public void Show(EventSO eventData)
        {
            if (eventData == null)
                return;

            if (panel != null)
                panel.SetActive(true);

            if (titleText != null)
                titleText.text = eventData.Title;

            if (descriptionText != null)
                descriptionText.text = eventData.Description;

            RebuildChoices(eventData.Choices);
        }

        public void Hide()
        {
            if (panel != null)
                panel.SetActive(false);

            ClearChoices();
        }

        private void RebuildChoices(EventChoiceData[] choices)
        {
            ClearChoices();

            if (choicesRoot == null || choicePrefab == null || choices == null)
                return;

            foreach (EventChoiceData choice in choices)
            {
                if (choice == null)
                    continue;

                EventChoiceView view = Instantiate(choicePrefab, choicesRoot);
                view.Setup(choice, OnChoiceClicked);
            }
        }

        private void ClearChoices()
        {
            if (choicesRoot == null)
                return;

            foreach (Transform child in choicesRoot)
                Destroy(child.gameObject);
        }

        private void OnChoiceClicked(EventChoiceData choice)
        {
            ChoiceClicked?.Invoke(choice);
        }
    }
}