using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoadOfAsh.Scripts.Presentation.Tutorial
{
    public class TutorialPanelView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Button nextButton;

        private Action _nextClicked;

        private void Awake()
        {
            if (nextButton != null)
            {
                nextButton.onClick.RemoveAllListeners();
                nextButton.onClick.AddListener(OnNextClicked);
            }
        }

        private void OnDestroy()
        {
            if (nextButton != null)
                nextButton.onClick.RemoveListener(OnNextClicked);
        }

        public void Show(string title, string description, bool showNextButton, Action nextClicked = null)
        {
            _nextClicked = nextClicked;

            if (panel != null)
                panel.SetActive(true);

            if (titleText != null)
                titleText.text = title;

            if (descriptionText != null)
                descriptionText.text = description;

            if (nextButton != null)
                nextButton.gameObject.SetActive(showNextButton);
        }

        public void Hide()
        {
            _nextClicked = null;

            if (panel != null)
                panel.SetActive(false);
        }

        private void OnNextClicked()
        {
            _nextClicked?.Invoke();
        }
    }
}