using UnityEngine;

namespace VehicleService
{
    public class BoatRockingEffect : MonoBehaviour
    {
        [SerializeField] private float rockAngle = 3f;
        [SerializeField] private float rockSpeed = 0.5f;

        private Quaternion _baseLocalRotation;
        private float _phaseOffset;

        private void Awake()
        {
            _baseLocalRotation = transform.localRotation;
            _phaseOffset = Random.Range(0f, Mathf.PI * 2f);
        }

        private void Update()
        {
            var roll = Mathf.Sin(Time.time * rockSpeed + _phaseOffset) * rockAngle;
            transform.localRotation = _baseLocalRotation * Quaternion.Euler(0f, 0f, roll);
        }
    }
}
