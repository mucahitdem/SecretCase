using Unity.Netcode;
using UnityEngine;

namespace GunService
{
    [RequireComponent(typeof(HitscanShooter))]
    public class GunComponent : NetworkBehaviour
    {
        [SerializeField] private WeaponDefinition weapon;
        [SerializeField] private HitscanShooter shooter;

        private float _lastFireTime;

        public WeaponDefinition Weapon => weapon;

        private void Awake()
        {
            GunViewModel.Attach(transform);
        }

        public bool TryFire(Vector3 origin, Vector3 direction, ulong instigatorClientId)
        {
            if (!IsServer) return false;
            if (weapon == null || shooter == null) return false;
            if (Time.time - _lastFireTime < 1f / weapon.FireRate) return false;

            _lastFireTime = Time.time;
            var hitSomething = shooter.TryFire(origin, direction, weapon.Range, weapon.Damage, instigatorClientId, out var hit);
            var endPoint = hitSomething ? hit.point : origin + direction * weapon.Range;

            GunActionManager.onFired?.Invoke(NetworkObject, origin, direction);
            if (hitSomething)
                GunActionManager.onHit?.Invoke(NetworkObject, hit.collider != null ? hit.collider.gameObject : null);

            PlayFireEffectClientRpc(endPoint, hitSomething);

            return hitSomething;
        }

        [ClientRpc]
        private void PlayFireEffectClientRpc(Vector3 endPoint, bool hitSomething)
        {
            var muzzlePosition = GunViewModel.GetMuzzlePosition(transform);

            if (weapon != null && weapon.FireSound != null)
                AudioSource.PlayClipAtPoint(weapon.FireSound, muzzlePosition);

            GunActionManager.onFireEffect?.Invoke(muzzlePosition, endPoint, hitSomething);
        }
    }
}
