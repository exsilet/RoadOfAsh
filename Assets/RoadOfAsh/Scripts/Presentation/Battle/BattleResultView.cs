using TMPro;
using UnityEngine;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class BattleResultView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text resultText;

        [Header("Texts")]
        [SerializeField] private string victoryText = "ПОБЕДА";
        [SerializeField] private string defeatText = "ПОРАЖЕНИЕ";

        public void Hide()
        {
            if (panel != null)
                panel.SetActive(false);
        }

        public void Show(bool playerWon)
        {
            if (panel != null)
                panel.SetActive(true);

            if (resultText != null)
            {
                resultText.text = playerWon
                    ? victoryText
                    : defeatText;
            }
        }
    }
}