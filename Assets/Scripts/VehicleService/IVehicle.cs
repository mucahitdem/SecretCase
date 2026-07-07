using System;
using UnityEngine;

namespace VehicleService
{
    public interface IVehicle
    {
        Transform DeckAnchor { get; }
        bool IsCaptured { get; }

        event Action<bool> CapturedChanged;

        void SetControlInput(float throttle, float steer);
        void SetCaptured(bool value);
    }
}
