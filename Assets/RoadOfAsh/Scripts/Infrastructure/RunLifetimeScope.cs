using RoadOfAsh.Scripts.Domain;
using RoadOfAsh.Scripts.Domain.Battle;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Distortion;
using RoadOfAsh.Scripts.Domain.Map;
using RoadOfAsh.Scripts.Domain.Players;
using RoadOfAsh.Scripts.Domain.Rewards;
using RoadOfAsh.Scripts.Domain.Shop;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace RoadOfAsh.Scripts.Infrastructure
{
    public class RunLifetimeScope : LifetimeScope
    {
        public static RunLifetimeScope Instance { get; private set; }

        protected override void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(new PlayerState());
            builder.RegisterInstance(new RunState());

            builder.Register<ICardService, CardService>(Lifetime.Singleton);
            builder.Register<IDistortionService, DistortionService>(Lifetime.Singleton);
            builder.Register<IBattleService, BattleService>(Lifetime.Singleton);
            builder.Register<IMapService, MapService>(Lifetime.Singleton);
            builder.Register<RewardService>(Lifetime.Singleton).As<IRewardService>();
            builder.Register<ShopService>(Lifetime.Singleton).As<IShopService>();
        }

        public static void LoadScene(string sceneName)
        {
            if (Instance == null)
            {
                Debug.LogError("RunLifetimeScope is missing. Start game from BootstrapScene.");
                return;
            }

            EnqueueParent(Instance);
            SceneManager.LoadScene(sceneName);
        }
    }
}