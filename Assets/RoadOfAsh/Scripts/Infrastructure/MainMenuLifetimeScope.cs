using RoadOfAsh.Scripts.Presentation.MainMenu;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace RoadOfAsh.Scripts.Infrastructure
{
    public class MainMenuLifetimeScope : LifetimeScope
    {
        [SerializeField] private MainMenuScreen mainMenuScreen;

        protected override void Configure(IContainerBuilder builder)
        {
            if (mainMenuScreen != null)
                builder.RegisterComponent(mainMenuScreen);
        }
    }
}