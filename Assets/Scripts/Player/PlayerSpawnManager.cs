using UnityEngine;

namespace Player
{
    public class PlayerSpawnManager : MonoBehaviour
    {
        [SerializeField]
        private Transform[] spawnPoints;

        private int _nextSpawnIndex;

        public bool TryGetNextSpawnPoint(out Vector3 position, out Quaternion rotation)
        {
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                position = Vector3.zero;
                rotation = Quaternion.identity;
                Debug.Log("SPAWN POINT IS NULL");
                return false;
            }

            var spawnPoint = spawnPoints[_nextSpawnIndex % spawnPoints.Length];
            _nextSpawnIndex++;

            position = spawnPoint.position;
            rotation = spawnPoint.rotation;
            Debug.Log("SPAWN POINT IS " + spawnPoint.transform.name);

            return true;
        }
    }
}