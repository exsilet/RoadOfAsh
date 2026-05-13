using System;
using RoadOfAsh.Scripts.Domain.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoadOfAsh.Scripts.Presentation.Map
{
    public class EventChoiceView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;

        private EventChoiceData _choice;
        private Action<EventChoiceData> _clicked;

        public void Setup(EventChoiceData choice, Action<EventChoiceData> clicked)
        {
            _choice = choice;
            _clicked = clicked;

            if (titleText != null)
                titleText.text = choice != null ? choice.Title : string.Empty;

            if (descriptionText != null)
                descriptionText.text = choice != null ? choice.Description : string.Empty;

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnClicked);
            }
        }

        private void OnClicked()
        {
            if (_choice == null)
                return;

            _clicked?.Invoke(_choice);
        }

        private void OnDestroy()
        {
            if (button != null)
                button.onClick.RemoveAllListeners();
        }
    }
}