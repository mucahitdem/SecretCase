using Unity.Netcode;
using UnityEngine;

namespace AIService
{
    public class AICrew : NetworkBehaviour
    {
        [SerializeField] private Transform[] wanderPoints;
        [SerializeField] private float moveSpeed = 1.5f;
        [SerializeField] private float waypointTolerance = 0.5f;

        private int _targetIndex;

        private void FixedUpdate()
        {
            if (!IsServer) return;
            if (wanderPoints == null || wanderPoints.Length == 0) return;

            var targetPoint = wanderPoints[_targetIndex].position;
            var toTarget = targetPoint - transform.position;
            toTarget.y = 0f;

            if (toTarget.magnitude <= waypointTolerance)
            {
                _targetIndex = (_targetIndex + 1) % wanderPoints.Length;
                return;
            }

            var direction = toTarget.normalized;
            transform.position += direction * moveSpeed * Time.fixedDeltaTime;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
    }
}
