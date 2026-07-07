using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VehicleService
{
    public class PlayerVehicleDriver : NetworkBehaviour
    {
        private IVehicle _vehicle;

        private void Awake()
        {
            _vehicle = GetComponent<IVehicle>();
        }

        private void Update()
        {
            if (!IsOwner || _vehicle == null || !_vehicle.IsCaptured)
                return;
            
            var keyboard = Keyboard.current;
            var throttle = 0f;
            var steer = 0f;

            if (keyboard != null)
            {
                if (keyboard.wKey.isPressed)
                    throttle += 1f;
                if (keyboard.sKey.isPressed)
                    throttle -= 1f;
                if (keyboard.dKey.isPressed)
                    steer += 1f;
                if (keyboard.aKey.isPressed)
                    steer -= 1f;
            }

            _vehicle.SetControlInput(throttle, steer);
        }
    }
}