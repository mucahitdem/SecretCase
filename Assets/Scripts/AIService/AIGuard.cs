using GunService;
using HealthService;
using Unity.Netcode;
using UnityEngine;

namespace AIService
{
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(GunComponent))]
    public class AIGuard : NetworkBehaviour
    {
        [SerializeField] private float alertRange = 15f;
        [SerializeField] private float combatRange = 10f;
        [SerializeField] private Transform muzzle;

        private HealthComponent _health;
        private GunComponent _gun;
        private AIGuardBrain _brain;
        private bool _tookDamageThisTick;
        private ulong _targetClientId;
        private Transform _targetPlayer;

        public override void OnNetworkSpawn()
        {
            _health = GetComponent<HealthComponent>();
            _gun = GetComponent<GunComponent>();
            _brain = new AIGuardBrain(alertRange, combatRange);

            _health.Damaged += HandleDamaged;
        }

        public override void OnNetworkDespawn()
        {
            _health.Damaged -= HandleDamaged;
        }

        private void HandleDamaged(float amount, ulong instigatorClientId)
        {
            _tookDamageThisTick = true;
            _targetClientId = instigatorClientId;

            if (NetworkManager.ConnectedClients.TryGetValue(instigatorClientId, out var client) && client.PlayerObject != null)
                _targetPlayer = client.PlayerObject.transform;
        }

        private void FixedUpdate()
        {
            if (!IsServer || _brain == null) return;
            if (!_health.IsAlive) return;

            FindNearestPlayerInRange();

            var distance = _targetPlayer != null ? Vector3.Distance(transform.position, _targetPlayer.position) : float.MaxValue;
            var newState = _brain.Evaluate(distance, _health.IsAlive, _tookDamageThisTick);
            _tookDamageThisTick = false;

            AIActionManager.onGuardStateChanged?.Invoke(NetworkObject, newState);

            if (newState == AIGuardState.Combat && _targetPlayer != null)
            {
                var direction = (_targetPlayer.position - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

                var origin = muzzle != null ? muzzle.position : transform.position;
                _gun.TryFire(origin, direction, _targetClientId);
            }
        }

        private void FindNearestPlayerInRange()
        {
            Transform nearestPlayer = null;
            var nearestDistance = alertRange;
            var nearestClientId = _targetClientId;

            foreach (var client in NetworkManager.ConnectedClientsList)
            {
                if (client.PlayerObject == null)
                    continue;

                var distance = Vector3.Distance(transform.position, client.PlayerObject.transform.position);
                if (distance > nearestDistance)
                    continue;

                nearestDistance = distance;
                nearestPlayer = client.PlayerObject.transform;
                nearestClientId = client.ClientId;
            }

            if (nearestPlayer != null)
            {
                _targetPlayer = nearestPlayer;
                _targetClientId = nearestClientId;
            }
            else if (_targetPlayer != null && Vector3.Distance(transform.position, _targetPlayer.position) > alertRange)
            {
                _targetPlayer = null;
            }
        }
    }
}
