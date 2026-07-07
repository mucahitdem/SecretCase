using Game.Core.Data;

namespace Game.Core.Interfaces
{
    public interface ISaveService
    {
        void Save(SaveData data);
        SaveData Load();
        bool HasSaveData();
    }
}
