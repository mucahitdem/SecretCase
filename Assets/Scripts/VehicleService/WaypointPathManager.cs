using System.Collections.Generic;
using UnityEngine;

namespace VehicleService
{
    public class WaypointPathManager : MonoBehaviour
    {
        public static WaypointPathManager Instance { get; private set; }

        private readonly Dictionary<WaypointPath, bool> _busyByPath = new Dictionary<WaypointPath, bool>();

        private void Awake()
        {
            Instance = this;

            foreach (var path in FindObjectsByType<WaypointPath>(FindObjectsSortMode.None))
                _busyByPath[path] = false;
        }

        public bool TryAcquirePath(out WaypointPath path)
        {
            var free = new List<WaypointPath>();
            foreach (var entry in _busyByPath)
            {
                if (!entry.Value)
                    free.Add(entry.Key);
            }

            if (free.Count == 0)
            {
                path = null;
                return false;
            }

            path = free[Random.Range(0, free.Count)];
            _busyByPath[path] = true;
            return true;
        }

        public void ReleasePath(WaypointPath path)
        {
            if (path != null && _busyByPath.ContainsKey(path))
                _busyByPath[path] = false;
        }
    }
}
