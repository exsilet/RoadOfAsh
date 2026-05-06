using System.Collections;
using System.Collections.Generic;
using RoadOfAsh.Scripts.Domain.Cards;
using TMPro;
using UnityEngine;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class CardResultView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text resultText;
        [SerializeField] private float hideDelay = 1.5f;

        [Header("Texts")]
        [SerializeField] private string corruptedText = "ИСКАЖЕНО";
        [SerializeField] private string noEnergyText = "Недостаточно энергии";
        [SerializeField] private string noEffectText = "Без эффекта";

        [Header("Effect Names")]
        [SerializeField] private string damageText = "Урон";
        [SerializeField] private string blockText = "Блок";
        [SerializeField] private string drawText = "Добор";
        [SerializeField] private string weakText = "Слабость";
        [SerializeField] private string poisonText = "Яд";
        [SerializeField] private string energyText = "Энергия";

        private Coroutine _hideRoutine;

        public void HideInstant()
        {
            StopHideRoutine();

            if (panel != null)
                panel.SetActive(false);
        }

        public void ShowCardResult(CardSO card, PlayedCardResult result)
        {
            if (card == null || result == null)
                return;

            if (panel != null)
                panel.SetActive(true);

            if (resultText != null)
            {
                string normalEffectsText = BuildEffectsText(card.Effects);
                string finalEffectsText = BuildEffectsText(result.FinalEffects);

                if (result.WasCorrupted)
                {
                    resultText.color = new Color(1f, 0.35f, 0.35f, 1f);

                    resultText.text =
                        $"{card.CardName}\n" +
                        $"{corruptedText}\n" +
                        $"{normalEffectsText} → {finalEffectsText}";
                }
                else
                {
                    resultText.color = Color.white;

                    resultText.text =
                        $"{card.CardName}\n" +
                        $"{finalEffectsText}";
                }
            }

            RestartHideRoutine();
        }

        public void ShowNotEnoughEnergy()
        {
            if (panel != null)
                panel.SetActive(true);

            if (resultText != null)
            {
                resultText.color = new Color(1f, 0.35f, 0.35f, 1f);
                resultText.text = noEnergyText;
            }

            RestartHideRoutine();
        }

        private void RestartHideRoutine()
        {
            StopHideRoutine();
            _hideRoutine = StartCoroutine(HideRoutine());
        }

        private IEnumerator HideRoutine()
        {
            yield return new WaitForSeconds(hideDelay);

            if (panel != null)
                panel.SetActive(false);

            _hideRoutine = null;
        }

        private void StopHideRoutine()
        {
            if (_hideRoutine == null)
                return;

            StopCoroutine(_hideRoutine);
            _hideRoutine = null;
        }

        private string BuildEffectsText(List<CardEffect> effects)
        {
            if (effects == null || effects.Count == 0)
                return noEffectText;

            List<string> parts = new();

            foreach (CardEffect effect in effects)
                parts.Add($"{GetEffectName(effect.Type)} {effect.Value}");

            return string.Join(", ", parts);
        }

        private string GetEffectName(EffectType type)
        {
            return type switch
            {
                EffectType.Damage => damageText,
                EffectType.Block => blockText,
                EffectType.Draw => drawText,
                EffectType.ApplyWeak => weakText,
                EffectType.ApplyPoison => poisonText,
                EffectType.GainEnergy => energyText,
                _ => type.ToString()
            };
        }
    }
}