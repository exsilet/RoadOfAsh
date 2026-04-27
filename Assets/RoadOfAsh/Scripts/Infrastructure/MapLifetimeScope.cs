using RoadOfAsh.Scripts.Presentation.Map;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace RoadOfAsh.Scripts.Infrastructure
{
    public class MapLifetimeScope : LifetimeScope
    {
        [SerializeField] private MapScreen mapScreen;

        protected override void Configure(IContainerBuilder builder)
        {
            if (mapScreen == null)
            {
                Debug.LogError("MapLifetimeScope: MapScreen is not assigned.");
                return;
            }

            builder.RegisterComponent(mapScreen);
        }
    }
}