using Unity.Netcode;
using UnityEngine;

namespace VehicleService
{
    public class AIVehicleDriver : NetworkBehaviour
    {
        [SerializeField]
        private WaypointPath path;

        [SerializeField]
        private float waypointTolerance = 4f;

        [SerializeField]
        private float turnSensitivity = 1.5f;

        [SerializeField]
        private float cruiseThrottle = 0.6f;

        private IVehicle _vehicle;
        private int _targetIndex;

        private void Awake()
        {
            _vehicle = GetComponent<IVehicle>();
        }

        public void StartPatrolling(WaypointPath newPath)
        {
            path = newPath;
            _targetIndex = 0;
            enabled = true;
        }

        public void StopPatrolling()
        {
            enabled = false;
            _vehicle?.SetControlInput(0f, 0f);
        }

        private void Update()
        {
            if (!IsOwner || _vehicle == null || _vehicle.IsCaptured)
                return;
            if (path == null || path.PointCount == 0)
                return;

            var targetPoint = path.GetPoint(_targetIndex);
            var toTarget = targetPoint - transform.position;
            toTarget.y = 0f;

            if (toTarget.magnitude <= waypointTolerance)
            {
                _targetIndex = (_targetIndex + 1) % path.PointCount;
                return;
            }

            var localTarget = transform.InverseTransformDirection(toTarget.normalized);
            var steer = Mathf.Clamp(localTarget.x * turnSensitivity, -1f, 1f);

            _vehicle.SetControlInput(cruiseThrottle, steer);
        }
    }
}