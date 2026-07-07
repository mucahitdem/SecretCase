using HealthService;
using UnityEngine;

namespace GunService
{
    public class HitscanShooter : MonoBehaviour
    {
        [SerializeField] private LayerMask hitMask = ~0;

        public bool TryFire(Vector3 origin, Vector3 direction, float range, float damage, ulong instigatorClientId, out RaycastHit hit)
        {
            if (Physics.Raycast(origin, direction, out hit, range, hitMask))
            {
                var damageable = hit.collider.GetComponentInParent<IDamageable>();
                damageable?.ApplyDamage(damage, instigatorClientId);
                return true;
            }

            return false;
        }
    }
}
