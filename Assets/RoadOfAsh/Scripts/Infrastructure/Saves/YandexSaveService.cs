using RoadOfAsh.Scripts.Domain;
using RoadOfAsh.Scripts.Domain.Players;
using UnityEngine;
using YG;

namespace RoadOfAsh.Scripts.Infrastructure.Saves
{
    public class YandexSaveService : ISaveService
    {
        private readonly PlayerState _playerState;
        private readonly RunState _runState;

        public bool HasSave => YG2.saves != null && YG2.saves.roadOfAshSave != null && YG2.saves.roadOfAshSave.HasSave;

        public YandexSaveService(PlayerState playerState, RunState runState)
        {
            _playerState = playerState;
            _runState = runState;
        }

        public void SaveRun()
        {
            if (YG2.saves == null)
                return;

            RoadOfAshSaveData save = YG2.saves.roadOfAshSave;

            if (save == null)
            {
                save = new RoadOfAshSaveData();
                YG2.saves.roadOfAshSave = save;
            }

            save.HasSave = true;
            save.IntroBattleCompleted = _runState.IntroBattleCompleted;
            save.Gold = _runState.Gold;
            save.SkippedRewards = _runState.SkippedRewards;
            save.PlayerHp = _playerState.HP;

            YG2.SaveProgress();

            Debug.Log("YandexSaveService: run saved.");
        }

        public bool TryLoadRun()
        {
            if (!HasSave)
                return false;

            RoadOfAshSaveData save = YG2.saves.roadOfAshSave;

            _runState.IntroBattleCompleted = save.IntroBattleCompleted;
            _runState.SetGold(save.Gold);
            _runState.SetSkippedRewards(save.SkippedRewards);

            _playerState.HP = Mathf.Clamp(save.PlayerHp, 1, _playerState.MaxHP);

            Debug.Log("YandexSaveService: run loaded.");
            return true;
        }

        public void ClearRun()
        {
            if (YG2.saves == null)
                return;

            YG2.saves.roadOfAshSave = new RoadOfAshSaveData();
            YG2.SaveProgress();

            Debug.Log("YandexSaveService: run save cleared.");
        }
    }
}