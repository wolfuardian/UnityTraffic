using UnityEngine;

namespace CrowdSample.Scripts.Utils
{
    public class SpatialUtils
    {
        public static Vector3 GetRandomPointInRadius(Vector3 center, float radius)
        {
            if (radius <= 0f)
            {
                return center;
            }

            var randomDirection = Random.insideUnitSphere * radius;
            randomDirection += center;
            return new Vector3(randomDirection.x, center.y, randomDirection.z);
        }
    }
}