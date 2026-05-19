using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class BattleCompletionView : MonoBehaviour
    {
        [SerializeField] private GameObject chapterCompletePanel;
        [SerializeField] private Button continueButton;

        public event Action ContinueClicked;

        private void Awake()
        {
            if (continueButton != null)
                continueButton.onClick.AddListener(() => ContinueClicked?.Invoke());

            Hide();
        }

        private void OnDestroy()
        {
            if (continueButton != null)
                continueButton.onClick.RemoveAllListeners();
        }

        public void Show()
        {
            if (chapterCompletePanel != null)
                chapterCompletePanel.SetActive(true);

            if (continueButton != null)
                continueButton.gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (chapterCompletePanel != null)
                chapterCompletePanel.SetActive(false);
        }
        
        public void SetContinueInteractable(bool interactable)
        {
            if (continueButton != null)
                continueButton.interactable = interactable;
        }
    }
}