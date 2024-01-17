﻿using UnityEngine;

namespace CrowdSample.Scripts.Runtime.Agent
{
    public class Path : MonoBehaviour
    {
        [SerializeField] private Vector3[] waypoints;
        public                   Vector3[] Waypoints => waypoints;

        public void SetWaypoints(Vector3[] newCheckpoints)
        {
            waypoints = newCheckpoints;
        }

        public void SetWaypointsElem(int index, Vector3 newCheckpoints)
        {
            waypoints[index] = newCheckpoints;
        }

        private float GetTotalLength()
        {
            if (waypoints == null || waypoints.Length < 2)
            {
                Debug.LogError("Insufficient checkpoints to form a path.");
                return 0;
            }

            float length = 0;
            for (var i = 0; i < waypoints.Length; i++)
            {
                var currentPoint = waypoints[i];
                var nextPoint    = (i == waypoints.Length - 1) ? waypoints[0] : waypoints[i + 1];
                length += Vector3.Distance(currentPoint, nextPoint);
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
            if (t >= 1) return waypoints[0]; // Return to the first point for a closed loop

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

            // Fallback, should not reach here in a closed loop
            return waypoints[0];
        }

        public Vector3 GetDirectionAt(float t)
        {
            if (waypoints == null || waypoints.Length < 2)
            {
                Debug.LogError("Path checkpoints are not set or insufficient.");
                return Vector3.zero;
            }

            // 當 t 為 0 時，返回從第一個點到第二個點的方向
            if (t <= 0) return (waypoints[1] - waypoints[0]).normalized;
            // 當 t 為 1 時，返回從最後一個點到第一個點的方向
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

            // Fallback, should not reach here in a closed loop
            return (waypoints[0] - waypoints[waypoints.Length - 1]).normalized;
        }
    }
}