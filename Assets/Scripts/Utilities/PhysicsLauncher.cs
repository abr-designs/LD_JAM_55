using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utilities
{
    [Serializable]
    public struct PhysicsLauncher
    {
        public Vector2 SpawnLocation => spawnLocation;
        [SerializeField]
        private Vector2 spawnLocation;
        [SerializeField]
        private Vector2 spawnDirection;
        [SerializeField, Min(0f)]
        private float spawnAngle;
        [SerializeField]
        private Vector2 spawnForceRange;
    
        public Vector2 GetLaunchVelocity()
        {
            var dir = Quaternion.Euler(0f, 0f, Random.Range(-spawnAngle, spawnAngle)) * spawnDirection.normalized;
            var force = Random.Range(spawnForceRange.x, spawnForceRange.y);

            return dir * force;
        }

#if UNITY_EDITOR

        public void DrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(spawnLocation, 0.25f);
            
            Gizmos.color =Color.green;
            Gizmos.DrawLine(spawnLocation, spawnLocation + spawnDirection.normalized);
            
            var dir1 = Quaternion.Euler(0f, 0f, -spawnAngle) * spawnDirection.normalized;
            var dir2 = Quaternion.Euler(0f, 0f, spawnAngle) * spawnDirection.normalized;
            
            Gizmos.color =Color.yellow;
            Gizmos.DrawLine(spawnLocation, spawnLocation + (Vector2)dir1.normalized);
            Gizmos.DrawLine(spawnLocation, spawnLocation + (Vector2)dir2.normalized);
        }
    
#endif
    
    }
}
