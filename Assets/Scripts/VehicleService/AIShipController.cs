using System.Collections.Generic;
using Core;
using HealthService;
using Unity.Netcode;
using UnityEngine;

namespace VehicleService
{
    [RequireComponent(typeof(AIVehicleDriver))]
    public class AIShipController : NetworkBehaviour
    {
        [SerializeField]
        private GameObject crewPrefab;

        [SerializeField]
        private Transform[] crewPoints;

        private IVehicle _vehicle;
        private AIVehicleDriver _driver;
        private WaypointPath _acquiredPath;
        private bool _allCrewDead;

        private readonly List<HealthComponent> _crewHealth = new List<HealthComponent>();

        private void Awake()
        {
            _vehicle = GetComponent<IVehicle>();
            _driver = GetComponent<AIVehicleDriver>();
        }

        private void OnEnable()
        {
            if (_vehicle != null)
                _vehicle.CapturedChanged += HandleCapturedChanged;
        }

        private void OnDisable()
        {
            if (_vehicle != null)
                _vehicle.CapturedChanged -= HandleCapturedChanged;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
                return;

            SpawnCrew();
            AcquirePath();
        }

        public override void OnNetworkDespawn()
        {
            foreach (var health in _crewHealth)
            {
                if (health != null)
                    health.Died -= HandleCrewDied;
            }
        }

        private void HandleCapturedChanged(bool captured)
        {
            if (!IsServer)
                return;

            if (captured)
                ReleasePath();
            else
                AcquirePath();
        }

        private void SpawnCrew()
        {
            if (crewPrefab == null || crewPoints == null)
                return;

            foreach (var point in crewPoints)
            {
                if (point == null)
                    continue;

                var crew = Instantiate(crewPrefab);
                var crewNetworkObject = crew.GetComponent<NetworkObject>();
                if (crewNetworkObject == null)
                    continue;

                crewNetworkObject.Spawn(true);
                NetworkAttachUtility.AttachToAnchor(crewNetworkObject, transform, point);

                var health = crew.GetComponent<HealthComponent>();
                if (health == null)
                    continue;

                _crewHealth.Add(health);
                health.Died += HandleCrewDied;
            }
        }

        private void HandleCrewDied()
        {
            if (_allCrewDead)
                return;

            foreach (var health in _crewHealth)
            {
                if (health != null && health.IsAlive)
                    return;
            }

            _allCrewDead = true;
            ReleasePath();
            _driver.StopPatrolling();
        }

        private void AcquirePath()
        {
            if (_allCrewDead || _acquiredPath != null || WaypointPathManager.Instance == null)
                return;

            if (WaypointPathManager.Instance.TryAcquirePath(out var path))
            {
                _acquiredPath = path;
                _driver.StartPatrolling(path);
            }
        }

        private void ReleasePath()
        {
            if (_acquiredPath == null || WaypointPathManager.Instance == null)
                return;

            WaypointPathManager.Instance.ReleasePath(_acquiredPath);
            _acquiredPath = null;
        }
    }
}
