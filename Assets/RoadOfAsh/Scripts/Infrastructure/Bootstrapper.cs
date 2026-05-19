using RoadOfAsh.Scripts.Domain;
using RoadOfAsh.Scripts.Infrastructure.Saves;
using UnityEngine;
using VContainer;

namespace RoadOfAsh.Scripts.Infrastructure
{
    public class Bootstrapper : MonoBehaviour
    {
        [SerializeField] private string tutorialSceneName = "TutorialBattleScene";
        [SerializeField] private string mapSceneName = "MapScene";

        private RunState _runState;
        private ISaveService _saveService;

        [Inject]
        public void Construct(RunState runState, ISaveService saveService)
        {
            _runState = runState;
            _saveService = saveService;
        }

        private void Start()
        {
            if (_saveService != null)
                _saveService.TryLoadRun();

            bool tutorialCompleted = _runState != null && _runState.IntroBattleCompleted;

            string targetScene = tutorialCompleted ? mapSceneName : tutorialSceneName;

            RunLifetimeScope.LoadScene(targetScene);
        }
    }
}