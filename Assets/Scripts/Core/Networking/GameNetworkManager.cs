using System;
using System.Collections.Generic;
using Game.Core.Networking;
using Player;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Core.Networking
{
    public class GameNetworkManager : MonoBehaviour
    {

        [SerializeField]
        private NetworkManager networkManager;

        [SerializeField]
        private UnityTransport transport;

        [SerializeField]
        private PlayerSpawnManager playerSpawnManager;

        private readonly Dictionary<ulong, string> _playerNames = new Dictionary<ulong, string>();

        public event Action<ulong> ClientConnected;
        public event Action<ulong> ClientDisconnected;
        public event Action ServerStarted;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            networkManager.NetworkConfig.ConnectionApproval = true;
        }

        private void OnEnable()
        {
            networkManager.OnClientConnectedCallback += HandleClientConnected;
            networkManager.OnClientDisconnectCallback += HandleClientDisconnected;
            networkManager.OnServerStarted += HandleServerStarted;
            networkManager.ConnectionApprovalCallback += HandleApproval;
        }

        private void OnDisable()
        {
            networkManager.OnClientConnectedCallback -= HandleClientConnected;
            networkManager.OnClientDisconnectCallback -= HandleClientDisconnected;
            networkManager.OnServerStarted -= HandleServerStarted;
            networkManager.ConnectionApprovalCallback -= HandleApproval;
        }

        public bool StartHost(string playerName)
        {
            ApplyConnectionPayload(playerName);
            return networkManager.StartHost();
        }

        public bool StartSolo(string playerName)
        {
            return StartHost(playerName);
        }

        public bool StartClient(string address, ushort port, string playerName)
        {
            transport.SetConnectionData(address, port);
            ApplyConnectionPayload(playerName);
            return networkManager.StartClient();
        }

        public void Shutdown()
        {
            networkManager.Shutdown();
        }

        public string GetPlayerName(ulong clientId)
        {
            return _playerNames.TryGetValue(clientId, out var name) ? name : "Player";
        }

        private void ApplyConnectionPayload(string playerName)
        {
            var payload = new ConnectionPayload { PlayerName = playerName };
            networkManager.NetworkConfig.ConnectionData = payload.ToBytes();
        }

        private void HandleApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            Debug.Log("SPAWN APPROVED");

            var payload = ConnectionPayload.FromBytes(request.Payload);
            _playerNames[request.ClientNetworkId] = payload.PlayerName;
          

            if (playerSpawnManager == null)
                Debug.Log("SPAWN MANAGER IS NULL");

            if (playerSpawnManager != null && playerSpawnManager.TryGetNextSpawnPoint(out var position, out var rotation))
            {
                response.Position = position;
                response.Rotation = rotation;
            }
            
            response.Approved = true;
            response.CreatePlayerObject = true;
        }

        private void HandleClientConnected(ulong clientId)
        {
            Debug.Log("CLIENT CONNECTED");
            ClientConnected?.Invoke(clientId);
        }

        private void HandleClientDisconnected(ulong clientId)
        {
            Debug.Log("CLIENT DISCONNECTED");
            _playerNames.Remove(clientId);
            ClientDisconnected?.Invoke(clientId);
        }

        private void HandleServerStarted()
        {
            Debug.Log("SERVER STARTED");
            ServerStarted?.Invoke();
        }
    }
}