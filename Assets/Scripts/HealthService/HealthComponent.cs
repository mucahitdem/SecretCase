using System;
using Unity.Netcode;
using UnityEngine;

namespace HealthService
{
    public class HealthComponent : NetworkBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 100f;

        private readonly NetworkVariable<float> _health = new NetworkVariable<float>(
            default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public event Action<float, ulong> Damaged;
        public event Action Died;

        public bool IsAlive => _health.Value > 0f;
        public float CurrentHealth => _health.Value;
        public float MaxHealth => maxHealth;

        public override void OnNetworkSpawn()
        {
            if (IsServer) _health.Value = maxHealth;
        }

        public void ApplyDamage(float amount, ulong instigatorClientId)
        {
            if (!IsServer) return;
            if (!IsAlive) return;

            _health.Value = Mathf.Max(0f, _health.Value - amount);
            Damaged?.Invoke(amount, instigatorClientId);
            HealthActionManager.onDamaged?.Invoke(NetworkObject, amount, instigatorClientId);

            if (_health.Value <= 0f)
            {
                Died?.Invoke();
                HealthActionManager.onDied?.Invoke(NetworkObject);
            }
        }
    }
}
