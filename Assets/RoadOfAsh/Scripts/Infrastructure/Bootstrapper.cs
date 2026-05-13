using RoadOfAsh.Scripts.Domain;
using UnityEngine;
using VContainer;

namespace RoadOfAsh.Scripts.Infrastructure
{
    public class Bootstrapper : MonoBehaviour
    {
        [SerializeField] private string tutorialSceneName = "TutorialBattleScene";
        [SerializeField] private string mapSceneName = "MapScene";

        private RunState _runState;

        [Inject]
        public void Construct(RunState runState)
        {
            _runState = runState;
        }

        private void Start()
        {
            if (_runState == null)
            {
                Debug.LogError("Bootstrapper: RunState was not injected.");
                return;
            }

            string targetScene = _runState.IntroBattleCompleted
                ? mapSceneName
                : tutorialSceneName;

            RunLifetimeScope.LoadScene(targetScene);
        }
    }
}