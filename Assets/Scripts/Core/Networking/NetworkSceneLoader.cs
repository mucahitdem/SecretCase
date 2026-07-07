using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Networking
{
    public class NetworkSceneLoader : MonoBehaviour
    {
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private string gameplaySceneName = "Island";

        public void LoadGameplayScene()
        {
            if (!networkManager.IsServer) return;
            networkManager.SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
        }
    }
}
