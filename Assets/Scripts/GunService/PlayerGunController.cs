using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GunService
{
    public class PlayerGunController : NetworkBehaviour
    {
        [SerializeField] private GunComponent gun;
        [SerializeField] private Camera playerCamera;

        private float _lastRequestTime;

        private void Update()
        {
            if (!IsOwner || playerCamera == null || gun == null) return;

            var mouse = Mouse.current;
            if (mouse == null || !mouse.leftButton.isPressed) return;

            var weapon = gun.Weapon;
            var cooldown = weapon != null ? 1f / weapon.FireRate : 0.25f;
            if (Time.time - _lastRequestTime < cooldown) return;

            _lastRequestTime = Time.time;
            FireServerRpc(playerCamera.transform.position, playerCamera.transform.forward);
        }

        [ServerRpc]
        private void FireServerRpc(Vector3 origin, Vector3 direction)
        {
            gun.TryFire(origin, direction, OwnerClientId);
        }
    }
}
