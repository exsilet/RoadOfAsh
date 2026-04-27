using RoadOfAsh.Scripts.Presentation.Battle;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace RoadOfAsh.Scripts.Infrastructure
{
    public class BattleLifetimeScope : LifetimeScope
    {
        [SerializeField] private BattleScreen battleScreen;

        protected override void Configure(IContainerBuilder builder)
        {
            if (battleScreen == null)
            {
                Debug.LogError("BattleLifetimeScope: BattleScreen is not assigned.");
                return;
            }

            builder.RegisterComponent(battleScreen);
        }
    }
}