using System.IO;
using Game.Core.Interfaces;
using Game.Core.Services;
using UnityEngine;

namespace Core.Bootstrap
{
    public class GameContext : MonoBehaviour
    {
        public static GameContext Current { get; private set; }

        public ISaveService SaveService { get; private set; }

        private void Awake()
        {
            if (Current != null && Current != this)
            {
                Destroy(gameObject);
                return;
            }

            Current = this;
            DontDestroyOnLoad(gameObject);

            IPersistence persistence = new FileSystemPersistence();
            var savePath = Path.Combine(Application.persistentDataPath, "save.json");
            SaveService = new JsonSaveService(persistence, savePath);
        }
    }
}
