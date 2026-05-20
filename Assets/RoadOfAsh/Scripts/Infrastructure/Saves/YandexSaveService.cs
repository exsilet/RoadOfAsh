using RoadOfAsh.Scripts.Domain;
using RoadOfAsh.Scripts.Domain.Cards;
using RoadOfAsh.Scripts.Domain.Map;
using RoadOfAsh.Scripts.Domain.Players;
using RoadOfAsh.Scripts.Domain.Relics;
using UnityEngine;
using YG;

namespace RoadOfAsh.Scripts.Infrastructure.Saves
{
    public class YandexSaveService : ISaveService
    {
        private readonly PlayerState _playerState;
        private readonly RunState _runState;
        private readonly IMapService _mapService;
        private readonly SaveContentDatabaseSO _contentDatabase;

        public bool HasSave => YG2.saves != null && YG2.saves.roadOfAshSave != null && YG2.saves.roadOfAshSave.HasSave;

        public YandexSaveService(PlayerState playerState, RunState runState, IMapService mapService, SaveContentDatabaseSO contentDatabase)
        {
            _playerState = playerState;
            _runState = runState;
            _mapService = mapService;
            _contentDatabase = contentDatabase;
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

            SaveDeck(save);
            SaveRelics(save);
            SaveMap(save);

            YG2.SaveProgress();

            Debug.Log(
                $"YandexSaveService: run saved. Deck={save.DeckCardIds.Count}, Relics={save.RelicIds.Count},CompletedNodes={save.CompletedNodeIds.Count}");
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

            RestoreDeck(save);
            RestoreRelics(save);

            Debug.Log(
                $"YandexSaveService: run loaded. Deck={_playerState.Deck.Count}, Relics={_playerState.Relics.Count}");
            return true;
        }

        public bool TryRestoreMap(MapSO mapConfig)
        {
            if (!HasSave)
                return false;

            if (mapConfig == null)
            {
                Debug.LogError("YandexSaveService: mapConfig is NULL. Cannot restore map.");
                return false;
            }

            RoadOfAshSaveData save = YG2.saves.roadOfAshSave;

            _mapService.RestoreMap(
                mapConfig,
                save.CurrentNodeId,
                save.SelectedNodeId,
                save.CompletedNodeIds);

            Debug.Log(
                $"YandexSaveService: map restored. Current={save.CurrentNodeId}, Selected={save.SelectedNodeId}, Completed={save.CompletedNodeIds.Count}");

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

        private void SaveDeck(RoadOfAshSaveData save)
        {
            save.DeckCardIds.Clear();

            foreach (CardSO card in _playerState.Deck)
            {
                if (card == null || string.IsNullOrWhiteSpace(card.Id))
                    continue;

                save.DeckCardIds.Add(card.Id);
            }
        }

        private void SaveRelics(RoadOfAshSaveData save)
        {
            save.RelicIds.Clear();

            foreach (RelicSO relic in _playerState.Relics)
            {
                if (relic == null || string.IsNullOrWhiteSpace(relic.Id))
                    continue;

                save.RelicIds.Add(relic.Id);
            }
        }

        private void SaveMap(RoadOfAshSaveData save)
        {
            save.CurrentNodeId = 0;
            save.SelectedNodeId = -1;
            save.CompletedNodeIds.Clear();

            if (_mapService == null || _mapService.State == null)
                return;

            save.CurrentNodeId = _mapService.State.CurrentNodeId;
            save.SelectedNodeId = _mapService.State.SelectedNodeId;

            foreach (int nodeId in _mapService.State.CompletedNodeIds)
                save.CompletedNodeIds.Add(nodeId);
        }

        private void RestoreDeck(RoadOfAshSaveData save)
        {
            _playerState.Hand.Clear();
            _playerState.Discard.Clear();
            _playerState.Deck.Clear();

            if (save.DeckCardIds == null || _contentDatabase == null)
                return;

            foreach (string cardId in save.DeckCardIds)
            {
                CardSO card = _contentDatabase.GetCardById(cardId);

                if (card != null)
                    _playerState.Deck.Add(card);
            }
        }

        private void RestoreRelics(RoadOfAshSaveData save)
        {
            _playerState.Relics.Clear();

            if (save.RelicIds == null || _contentDatabase == null)
                return;

            foreach (string relicId in save.RelicIds)
            {
                RelicSO relic = _contentDatabase.GetRelicById(relicId);

                if (relic != null)
                    _playerState.Relics.Add(relic);
            }
        }
    }
}