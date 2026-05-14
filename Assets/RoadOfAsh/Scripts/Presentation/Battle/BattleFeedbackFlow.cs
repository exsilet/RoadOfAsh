using RoadOfAsh.Scripts.Domain.Battle;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Relics;
using RoadOfAsh.Scripts.Presentation.Relics;
using UnityEngine;

namespace RoadOfAsh.Scripts.Presentation.Battle
{
    public class BattleFeedbackFlow : MonoBehaviour
    {
        [SerializeField] private BattleEffectsView battleEffectsView;
        [SerializeField] private CardResultView cardResultView;
        [SerializeField] private RelicBarView relicBarView;

        private IBattleService _battleService;
        private IRelicService _relicService;

        public void Initialize(IBattleService battleService, IRelicService relicService)
        {
            _battleService = battleService;
            _relicService = relicService;

            if (cardResultView != null)
                cardResultView.HideInstant();

            Subscribe();
        }

        public void Dispose()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            if (_battleService != null)
            {
                _battleService.OnCardPlayed += OnCardPlayed;

                if (battleEffectsView != null)
                {
                    _battleService.OnEnemyDamaged += battleEffectsView.ShowEnemyDamage;
                    _battleService.OnPlayerDamaged += battleEffectsView.ShowPlayerDamage;
                    _battleService.OnPlayerBlocked += battleEffectsView.ShowPlayerBlock;
                    _battleService.OnEnemyPoisonTick += battleEffectsView.ShowEnemyPoison;

                    _battleService.OnPlayerPoisoned += battleEffectsView.ShowPlayerPoison;
                    _battleService.OnPlayerWeakened += battleEffectsView.ShowPlayerWeak;
                    _battleService.OnEnemyHealed += battleEffectsView.ShowEnemyHeal;
                    _battleService.OnEnemyCleansed += battleEffectsView.ShowEnemyCleanse;
                }
            }

            if (_relicService != null)
                _relicService.RelicActivated += OnRelicActivated;
        }

        private void Unsubscribe()
        {
            if (_battleService != null)
            {
                _battleService.OnCardPlayed -= OnCardPlayed;

                if (battleEffectsView != null)
                {
                    _battleService.OnEnemyDamaged -= battleEffectsView.ShowEnemyDamage;
                    _battleService.OnPlayerDamaged -= battleEffectsView.ShowPlayerDamage;
                    _battleService.OnPlayerBlocked -= battleEffectsView.ShowPlayerBlock;
                    _battleService.OnEnemyPoisonTick -= battleEffectsView.ShowEnemyPoison;

                    _battleService.OnPlayerPoisoned -= battleEffectsView.ShowPlayerPoison;
                    _battleService.OnPlayerWeakened -= battleEffectsView.ShowPlayerWeak;
                    _battleService.OnEnemyHealed -= battleEffectsView.ShowEnemyHeal;
                    _battleService.OnEnemyCleansed -= battleEffectsView.ShowEnemyCleanse;
                }
            }

            if (_relicService != null)
                _relicService.RelicActivated -= OnRelicActivated;
        }

        private void OnCardPlayed(CardSO card, PlayedCardResult result)
        {
            if (cardResultView != null)
                cardResultView.ShowCardResult(card, result);
        }

        private void OnRelicActivated(RelicSO relic)
        {
            if (relicBarView != null)
                relicBarView.PlayRelicActivated(relic);

            if (battleEffectsView != null &&
                relic != null &&
                relic.EffectType == RelicEffectType.BlockFirstDistortionEachTurn)
            {
                battleEffectsView.ShowRelicBlockedDistortion();
            }
        }
    }
}