using Core.Bootstrap;
using InventoryService;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class SaveLoadController : NetworkBehaviour
    {
        [SerializeField] private PlayerInventoryController inventory;

        private void Update()
        {
            if (!IsOwner) return;

            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.f5Key.wasPressedThisFrame) SaveServerRpc();
            if (keyboard.f9Key.wasPressedThisFrame) LoadServerRpc();
        }

        [ServerRpc]
        private void SaveServerRpc()
        {
            if (GameContext.Current == null) return;
            GameContext.Current.SaveService.Save(inventory.ToSaveData());
        }

        [ServerRpc]
        private void LoadServerRpc()
        {
            if (GameContext.Current == null) return;
            if (!GameContext.Current.SaveService.HasSaveData()) return;
            inventory.LoadFrom(GameContext.Current.SaveService.Load());
        }
    }
}
