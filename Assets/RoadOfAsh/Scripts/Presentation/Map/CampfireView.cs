using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoadOfAsh.Scripts.Presentation.Map
{
    public class CampfireView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text healText;
        [SerializeField] private Button healButton;
        [SerializeField] private Button closeButton;

        [Header("Text")]
        [SerializeField] private string healFormat = "+{0} HP";

        public event Action HealClicked;
        public event Action CloseClicked;

        private void Awake()
        {
            Hide();

            if (healButton != null)
                healButton.onClick.AddListener(OnHealClicked);

            if (closeButton != null)
                closeButton.onClick.AddListener(OnCloseClicked);
        }

        private void OnDestroy()
        {
            if (healButton != null)
                healButton.onClick.RemoveListener(OnHealClicked);

            if (closeButton != null)
                closeButton.onClick.RemoveListener(OnCloseClicked);
        }

        public void Show(int healAmount)
        {
            if (panel != null)
                panel.SetActive(true);

            if (healText != null)
                healText.text = string.Format(healFormat, healAmount);
        }

        public void Hide()
        {
            if (panel != null)
                panel.SetActive(false);
        }

        private void OnHealClicked()
        {
            HealClicked?.Invoke();
        }

        private void OnCloseClicked()
        {
            CloseClicked?.Invoke();
        }
    }
}