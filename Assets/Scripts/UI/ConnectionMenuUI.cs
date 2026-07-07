using Core.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ConnectionMenuUI : MonoBehaviour
    {
        [SerializeField]
        private GameNetworkManager networkManager;

        [SerializeField]
        private TMP_InputField playerNameInput;

        [SerializeField]
        private TMP_InputField addressInput;

        [SerializeField]
        private TMP_InputField portInput;

        [SerializeField]
        private Button hostButton;

        [SerializeField]
        private Button soloButton;

        [SerializeField]
        private Button joinButton;

        [SerializeField]
        private GameObject rootPanel;

        private void Awake()
        {
            hostButton.onClick.AddListener(OnHostClicked);
            soloButton.onClick.AddListener(OnSoloClicked);
            joinButton.onClick.AddListener(OnJoinClicked);
        }

        private void OnHostClicked()
        {
            networkManager.StartHost(ResolvePlayerName());
            Hide();
        }

        private void OnSoloClicked()
        {
            networkManager.StartSolo(ResolvePlayerName());
            Hide();
        }

        private void OnJoinClicked()
        {
            var address = string.IsNullOrWhiteSpace(addressInput.text) ? "127.0.0.1" : addressInput.text;
            var port = ushort.TryParse(portInput.text, out var parsedPort) ? parsedPort : (ushort)7777;
            networkManager.StartClient(address, port, ResolvePlayerName());
            Hide();
        }

        private string ResolvePlayerName()
        {
            return string.IsNullOrWhiteSpace(playerNameInput.text) ? "Player" : playerNameInput.text;
        }

        private void Hide()
        {
            rootPanel.SetActive(false);
        }
    }
}