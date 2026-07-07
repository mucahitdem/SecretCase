using Unity.Netcode;

namespace VehicleService
{
    public class PlayerVehicleTracker : NetworkBehaviour
    {
        private readonly NetworkVariable<NetworkObjectReference> _currentVehicle = new NetworkVariable<NetworkObjectReference>();

        public void SetCurrentVehicle(NetworkObject vehicleObject)
        {
            if (!IsServer) return;
            _currentVehicle.Value = vehicleObject;
        }

        public bool TryGetCurrentVehicle(out NetworkObject vehicleObject)
        {
            return _currentVehicle.Value.TryGet(out vehicleObject);
        }
    }
}
