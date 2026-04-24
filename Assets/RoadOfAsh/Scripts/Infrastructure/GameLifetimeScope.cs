using RoadOfAsh.Scripts.Domain.Battle;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Players;
using RoadOfAsh.Scripts.Presentation.Battle;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace RoadOfAsh.Scripts.Infrastructure
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private BattleScreen battleScreen;
        
        protected override void Configure(IContainerBuilder builder)
        {
            var playerState = new PlayerState();
            builder.RegisterInstance(playerState);

            builder.Register<ICardService, CardService>(Lifetime.Singleton);
            builder.Register<IBattleService, BattleService>(Lifetime.Singleton);
            
            if (battleScreen != null)
                builder.RegisterComponent(battleScreen);
        }
    }
}