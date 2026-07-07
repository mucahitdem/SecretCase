using UnityEngine;

namespace VehicleService
{
    public class WaypointPath : MonoBehaviour
    {
        [SerializeField] private Transform[] points;

        public int PointCount => points != null ? points.Length : 0;

        public Vector3 GetPoint(int index)
        {
            return points[index].position;
        }
    }
}
