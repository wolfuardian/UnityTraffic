using System;
using UnityEngine;

namespace CrowdSample.Scripts.Runtime.Agent
{
    public class Path : MonoBehaviour
    {
        [SerializeField] private Vector3[] waypoints;
        public                   Vector3[] Waypoints => waypoints;
        public                   bool      isClosed;


        public void SetWaypoints(Vector3[] newCheckpoints)
        {
            waypoints = newCheckpoints;
        }

        public void SetWaypointsElem(int index, Vector3 newCheckpoints)
        {
            waypoints[index] = newCheckpoints;
        }

        public float GetTotalLength()
        {
            if (waypoints == null || waypoints.Length < 2)
            {
                Debug.LogError("Insufficient checkpoints to form a path.");
                return 0;
            }

            float length = 0;
            for (var i = 0; i < waypoints.Length - 1; i++)
            {
                var currentPoint = waypoints[i];
                var nextPoint    = waypoints[i + 1];
                length += Vector3.Distance(currentPoint, nextPoint);
            }

            if (isClosed && waypoints.Length > 2)
            {
                length += Vector3.Distance(waypoints[waypoints.Length - 1], waypoints[0]);
            }

            return length;
        }


        public Vector3 GetPointAt(float t)
        {
            if (waypoints == null || waypoints.Length < 2)
            {
                Debug.LogError("Path checkpoints are not set or insufficient.");
                return Vector3.zero;
            }

            if (t <= 0) return waypoints[0];
            if (t >= 1) return waypoints[0];

            var   totalLength      = GetTotalLength();
            var   targetLength     = t * totalLength;
            float cumulativeLength = 0;

            for (var i = 0; i < waypoints.Length; i++)
            {
                var currentPoint  = waypoints[i];
                var nextPoint     = (i == waypoints.Length - 1) ? waypoints[0] : waypoints[i + 1];
                var segment       = nextPoint - currentPoint;
                var segmentLength = segment.magnitude;

                if (cumulativeLength + segmentLength >= targetLength)
                {
                    var remainingLength = targetLength - cumulativeLength;
                    return currentPoint + segment.normalized * remainingLength;
                }

                cumulativeLength += segmentLength;
            }

            return waypoints[0];
        }

        public Vector3 GetDirectionAt(float t)
        {
            if (waypoints == null || waypoints.Length < 2)
            {
                Debug.LogError("Path checkpoints are not set or insufficient.");
                return Vector3.zero;
            }

            if (t <= 0) return (waypoints[1] - waypoints[0]).normalized;
            if (t >= 1) return (waypoints[0] - waypoints[waypoints.Length - 1]).normalized;

            var   totalLength      = GetTotalLength();
            var   targetLength     = t * totalLength;
            float cumulativeLength = 0;

            for (var i = 0; i < waypoints.Length; i++)
            {
                var currentPoint  = waypoints[i];
                var nextPoint     = (i == waypoints.Length - 1) ? waypoints[0] : waypoints[i + 1];
                var segment       = nextPoint - currentPoint;
                var segmentLength = segment.magnitude;

                if (cumulativeLength + segmentLength >= targetLength)
                {
                    return segment.normalized;
                }

                cumulativeLength += segmentLength;
            }

            return (waypoints[0] - waypoints[waypoints.Length - 1]).normalized;
        }

        public Vector3 GetPointAtDistance(float distance)
        {
            if (waypoints == null || waypoints.Length < 2)
            {
                Debug.LogError("Path checkpoints are not set or insufficient.");
                return Vector3.zero;
            }

            var totalLength                      = GetTotalLength();
            if (distance < 0) distance           = 0;
            if (distance > totalLength) distance = totalLength;

            float cumulativeLength = 0;

            for (var i = 0; i < waypoints.Length; i++)
            {
                var currentPoint  = waypoints[i];
                var nextPoint     = (i == waypoints.Length - 1) ? waypoints[0] : waypoints[i + 1];
                var segment       = nextPoint - currentPoint;
                var segmentLength = segment.magnitude;

                if (cumulativeLength + segmentLength >= distance)
                {
                    var remainingLength = distance - cumulativeLength;
                    return currentPoint + segment.normalized * remainingLength;
                }

                cumulativeLength += segmentLength;
            }

            return waypoints[0];
        }

        public Vector3 GetDirectionAtDistance(float distance)
        {
            if (Waypoints.Length < 2)
            {
                Debug.LogError("Insufficient waypoints to calculate direction.");
                return Vector3.forward;
            }

            var totalLength = 0f;
            for (var i = 0; i < Waypoints.Length; i++)
            {
                var startPoint    = Waypoints[i];
                var endPoint      = Waypoints[(i + 1) % Waypoints.Length];
                var segmentLength = Vector3.Distance(startPoint, endPoint);

                if (totalLength + segmentLength >= distance)
                {
                    return (endPoint - startPoint).normalized;
                }

                totalLength += segmentLength;
                if (totalLength > distance) break;
            }

            // 如果路徑封閉，從最後一個點指向第一個點
            return isClosed
                ? (Waypoints[0] - Waypoints[Waypoints.Length - 1]).normalized
                : (Waypoints[Waypoints.Length - 1] - Waypoints[Waypoints.Length - 2]).normalized;
        }
    }
}