using System;
using UnityEngine;

namespace CrowdSample.Scripts.Runtime.Agent
{
    public class Path : MonoBehaviour
    {
        [SerializeField] private Transform[] waypoints;
        [SerializeField] private Vector3[]   positions;

        public Transform[] Waypoints => waypoints;
        public Vector3[]   Positions => positions;

        public bool isClosed;

        public void SetWaypoints(Transform[] newCheckpoints)
        {
            waypoints = newCheckpoints;
        }

        public void SetWaypointsElem(int index, Transform newCheckpoints)
        {
            waypoints[index] = newCheckpoints;
        }

        public void SetPositions(Vector3[] newPositions)
        {
            positions = newPositions;
        }

        public void SetPositionsElem(int index, Vector3 newPosition)
        {
            positions[index] = newPosition;
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
                length += Vector3.Distance(currentPoint.position, nextPoint.position);
            }

            if (isClosed && waypoints.Length > 2)
            {
                length += Vector3.Distance(waypoints[waypoints.Length - 1].position, waypoints[0].position);
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

            if (t <= 0) return waypoints[0].position;
            if (t >= 1) return waypoints[0].position;

            var   totalLength      = GetTotalLength();
            var   targetLength     = t * totalLength;
            float cumulativeLength = 0;

            for (var i = 0; i < waypoints.Length; i++)
            {
                var currentPos    = waypoints[i].position;
                var nextPos       = (i == waypoints.Length - 1) ? waypoints[0].position : waypoints[i + 1].position;
                var segment       = nextPos - currentPos;
                var segmentLength = segment.magnitude;

                if (cumulativeLength + segmentLength >= targetLength)
                {
                    var remainingLength = targetLength - cumulativeLength;
                    return currentPos + segment.normalized * remainingLength;
                }

                cumulativeLength += segmentLength;
            }

            return waypoints[0].position;
        }

        public Vector3 GetDirectionAt(float t)
        {
            if (waypoints == null || waypoints.Length < 2)
            {
                Debug.LogError("Path checkpoints are not set or insufficient.");
                return Vector3.zero;
            }

            if (t <= 0) return (waypoints[1].position - waypoints[0].position).normalized;
            if (t >= 1) return (waypoints[0].position - waypoints[waypoints.Length - 1].position).normalized;

            var   totalLength      = GetTotalLength();
            var   targetLength     = t * totalLength;
            float cumulativeLength = 0;

            for (var i = 0; i < waypoints.Length; i++)
            {
                var currentPos    = waypoints[i].position;
                var nextPos       = (i == waypoints.Length - 1) ? waypoints[0].position : waypoints[i + 1].position;
                var segment       = nextPos - currentPos;
                var segmentLength = segment.magnitude;

                if (cumulativeLength + segmentLength >= targetLength)
                {
                    return segment.normalized;
                }

                cumulativeLength += segmentLength;
            }

            return (waypoints[0].position - waypoints[waypoints.Length - 1].position).normalized;
        }

        private void OnValidate()
        {
            SetPositions(new Vector3[waypoints.Length]);
            for (var i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] == null) continue;
                SetPositionsElem(i, waypoints[i].position);
            }
        }
    }
}