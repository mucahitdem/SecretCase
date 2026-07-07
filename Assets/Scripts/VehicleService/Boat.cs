using System;
using Unity.Netcode;
using UnityEngine;

namespace VehicleService
{
    [RequireComponent(typeof(Rigidbody))]
    public class Boat : NetworkBehaviour, IVehicle
    {
        [SerializeField]
        private float enginePower = 8f;

        [SerializeField]
        private float turnPower = 60f;

        [SerializeField]
        private float turnAcceleration = 90f;

        [SerializeField]
        private float fullTurnSpeed = 4f;

        [SerializeField]
        private float lateralGrip = 5f;

        [SerializeField]
        private Transform deckAnchor;

        private Rigidbody _rigidbody;
        private float _currentTurnRate;

        private readonly NetworkVariable<float> _throttleInput = new NetworkVariable<float>(
            default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        private readonly NetworkVariable<float> _steerInput = new NetworkVariable<float>(
            default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        private readonly NetworkVariable<bool> _isCaptured = new NetworkVariable<bool>(
            false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public Transform DeckAnchor => deckAnchor;
        public bool IsCaptured => _isCaptured.Value;

        public event Action<bool> CapturedChanged;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
                return;

            ApplyDriveMode(_isCaptured.Value);
        }

        public void SetControlInput(float throttle, float steer)
        {
            if (!IsOwner)
                return;
            _throttleInput.Value = Mathf.Clamp(throttle, -1f, 1f);
            _steerInput.Value = Mathf.Clamp(steer, -1f, 1f);
        }

        public void SetCaptured(bool value)
        {
            if (!IsServer)
                return;

            _isCaptured.Value = value;
            ApplyDriveMode(value);

            if (value)
                VehicleActionManager.onCaptured?.Invoke(NetworkObject);

            CapturedChanged?.Invoke(value);
        }

        private void ApplyDriveMode(bool playerControlled)
        {
            _rigidbody.isKinematic = !playerControlled;
        }

        private void FixedUpdate()
        {
            if (!IsServer)
                return;

            if (_isCaptured.Value)
                DriveWithRigidbody();
            else
                DriveWithTransform();
        }

        private void DriveWithRigidbody()
        {
            var forward = transform.forward * (_throttleInput.Value * enginePower);
            _rigidbody.AddForce(forward, ForceMode.Acceleration);

            var forwardSpeed = Vector3.Dot(_rigidbody.linearVelocity, transform.forward);
            var speedFactor = Mathf.Clamp01(Mathf.Abs(forwardSpeed) / fullTurnSpeed);
            var targetTurnRate = _steerInput.Value * turnPower * speedFactor;
            _currentTurnRate = Mathf.MoveTowards(_currentTurnRate, targetTurnRate, turnAcceleration * Time.fixedDeltaTime);

            var turn = _currentTurnRate * Time.fixedDeltaTime;
            _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(0f, turn, 0f));

            CancelLateralSlide();
        }

        private void CancelLateralSlide()
        {
            var localVelocity = transform.InverseTransformDirection(_rigidbody.linearVelocity);
            localVelocity.x = Mathf.MoveTowards(localVelocity.x, 0f, lateralGrip * Time.fixedDeltaTime);
            _rigidbody.linearVelocity = transform.TransformDirection(localVelocity);
        }

        private void DriveWithTransform()
        {
            var turn = _steerInput.Value * turnPower * Time.fixedDeltaTime;
            transform.Rotate(Vector3.up, turn, Space.World);

            var motion = transform.forward * (_throttleInput.Value * enginePower * Time.fixedDeltaTime);
            transform.position += motion;
        }
    }
}
