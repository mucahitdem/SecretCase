using Game.Core.Data;
using Game.Core.Interfaces;
using UnityEngine;

namespace Game.Core.Services
{
    public class JsonSaveService : ISaveService
    {
        private readonly IPersistence _persistence;
        private readonly string _savePath;

        public JsonSaveService(IPersistence persistence, string savePath)
        {
            _persistence = persistence;
            _savePath = savePath;
        }

        public void Save(SaveData data)
        {
            var json = JsonUtility.ToJson(data, true);
            _persistence.WriteAllText(_savePath, json);
        }

        public SaveData Load()
        {
            if (!_persistence.Exists(_savePath)) return new SaveData();

            var json = _persistence.ReadAllText(_savePath);
            return JsonUtility.FromJson<SaveData>(json);
        }

        public bool HasSaveData() => _persistence.Exists(_savePath);
    }
}
