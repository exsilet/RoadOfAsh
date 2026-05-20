using RoadOfAsh.Scripts.Domain.Map;

namespace RoadOfAsh.Scripts.Infrastructure.Saves
{
    public interface ISaveService
    {
        bool HasSave { get; }

        void SaveRun();
        bool TryLoadRun();
        bool TryRestoreMap(MapSO mapConfig);
        void ClearRun();
    }
}