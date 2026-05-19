namespace RoadOfAsh.Scripts.Infrastructure.Saves
{
    public interface ISaveService
    {
        bool HasSave { get; }

        void SaveRun();
        bool TryLoadRun();
        void ClearRun();
    }
}