using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public static class PathResolver
    {
        public static float GetTotalLength(List<Transform> waypoints, bool isClosedPath)
        {
            if (!ValidateWaypoints(waypoints)) return 0;

            var length = CalculatePathLength(waypoints);

            if (isClosedPath)
            {
                length += Vector3.Distance(waypoints[waypoints.Count - 1].position, waypoints[0].position);
            }

            return length;
        }

        public static float GetTotalLength(List<Vector3> waypoints, bool isClosedPath)
        {
            if (!ValidateWaypoints(waypoints)) return 0;

            var length = CalculatePathLength(waypoints);

            if (isClosedPath)
            {
                length += Vector3.Distance(waypoints[waypoints.Count - 1], waypoints[0]);
            }

            return length;
        }

        public static Vector3 GetPositionAt(List<Transform> waypoints, bool isClosedPath, float t)
        {
            if (!ValidateWaypoints(waypoints)) return Vector3.zero;

            if (t <= 0) return waypoints[0].position;
            if (t >= 1) return waypoints[waypoints.Count - 1].position;

            var totalLength  = GetTotalLength(waypoints, isClosedPath);
            var targetLength = t * totalLength;
            return CalculatePositionAlongPath(waypoints, targetLength, isClosedPath);
        }

        public static Vector3 GetPositionAt(List<Vector3> waypoints, bool isClosedPath, float t)
        {
            if (!ValidateWaypoints(waypoints)) return Vector3.zero;

            if (t <= 0) return waypoints[0];
            if (t >= 1) return waypoints[waypoints.Count - 1];

            var totalLength  = GetTotalLength(waypoints, isClosedPath);
            var targetLength = t * totalLength;
            return CalculatePositionAlongPath(waypoints, targetLength, isClosedPath);
        }

        public static Vector3 GetDirectionAt(List<Transform> waypoints, bool isClosedPath, float t)
        {
            if (!ValidateWaypoints(waypoints)) return Vector3.zero;

            if (t <= 0) return (waypoints[1].position - waypoints[0].position).normalized;
            if (t >= 1) return (waypoints[waypoints.Count - 1].position - waypoints[0].position).normalized;

            var totalLength  = GetTotalLength(waypoints, isClosedPath);
            var targetLength = t * totalLength;
            return CalculateDirectionAlongPath(waypoints, targetLength, isClosedPath);
        }

        public static Vector3 GetDirectionAt(List<Vector3> waypoints, bool isClosedPath, float t)
        {
            if (!ValidateWaypoints(waypoints)) return Vector3.zero;

            if (t <= 0) return (waypoints[1] - waypoints[0]).normalized;
            if (t >= 1) return (waypoints[waypoints.Count - 1] - waypoints[0]).normalized;

            var totalLength  = GetTotalLength(waypoints, isClosedPath);
            var targetLength = t * totalLength;
            return CalculateDirectionAlongPath(waypoints, targetLength, isClosedPath);
        }


        public static Vector3 GetRotationAt(List<Transform> waypoints, bool isClosedPath, float t)
        {
            if (!ValidateWaypoints(waypoints)) return Vector3.zero;

            if (t <= 0)
            {
                return Quaternion.LookRotation(waypoints[1].position - waypoints[0].position).eulerAngles;
            }

            if (t >= 1)
            {
                return Quaternion
                    .LookRotation(waypoints[waypoints.Count - 1].position - waypoints[waypoints.Count - 2].position)
                    .eulerAngles;
            }

            var     totalLength  = GetTotalLength(waypoints, isClosedPath);
            var     targetLength = t * totalLength;
            Vector3 direction    = CalculateDirectionAlongPath(waypoints, targetLength, isClosedPath);

            return Quaternion.LookRotation(direction).eulerAngles;
        }

        private static bool ValidateWaypoints(ICollection waypoints)
        {
            if (waypoints == null || waypoints.Count < 2)
            {
                Debug.LogError("Path waypoints are not set or insufficient.");
                return false;
            }

            return true;
        }

        private static bool ValidateWaypoints(List<Vector3> waypoints)
        {
            if (waypoints == null || waypoints.Count < 2)
            {
                Debug.LogError("Path waypoints are not set or insufficient.");
                return false;
            }

            return true;
        }

        private static float CalculatePathLength(IReadOnlyList<Transform> waypoints)
        {
            float length = 0;
            for (var i = 0; i < waypoints.Count - 1; i++)
            {
                length += Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);
            }

            return length;
        }

        private static float CalculatePathLength(IReadOnlyList<Vector3> waypoints)
        {
            float length = 0;
            for (var i = 0; i < waypoints.Count - 1; i++)
            {
                length += Vector3.Distance(waypoints[i], waypoints[i + 1]);
            }

            return length;
        }

        private static Vector3 CalculatePositionAlongPath(List<Transform> waypoints, float length,
            bool                                                          closedLoop)
        {
            float cumulativeLength = 0;

            for (var i = 0; i < waypoints.Count - 1; i++)
            {
                var segment       = waypoints[i + 1].position - waypoints[i].position;
                var segmentLength = segment.magnitude;

                if (cumulativeLength + segmentLength >= length)
                {
                    var remainingLength = length - cumulativeLength;
                    return waypoints[i].position + segment.normalized * remainingLength;
                }

                cumulativeLength += segmentLength;
            }

            if (closedLoop)
            {
                var finalSegment = waypoints[0].position - waypoints[waypoints.Count - 1].position;
                return waypoints[waypoints.Count - 1].position +
                       finalSegment.normalized * (length - cumulativeLength);
            }

            return waypoints[waypoints.Count - 1].position;
        }

        private static Vector3 CalculatePositionAlongPath(IReadOnlyList<Vector3> pts, float length, bool closedLoop)
        {
            float cumulativeLength = 0;

            for (var i = 0; i < pts.Count - 1; i++)
            {
                var segment = pts[i + 1] - pts[i];

                if (cumulativeLength + segment.magnitude >= length)
                {
                    var remainingLength = length - cumulativeLength;
                    return pts[i] + segment.normalized * remainingLength;
                }

                cumulativeLength += segment.magnitude;
            }

            if (!closedLoop) return pts[pts.Count - 1];

            var finalSegment = pts[0] - pts[pts.Count - 1];
            return pts[pts.Count - 1] +
                   finalSegment.normalized * (length - cumulativeLength);
        }

        private static Vector3 CalculateDirectionAlongPath(List<Transform> waypoints, float length,
            bool                                                           closedLoop)
        {
            float cumulativeLength = 0;

            for (var i = 0; i < waypoints.Count - 1; i++)
            {
                var segment       = waypoints[i + 1].position - waypoints[i].position;
                var segmentLength = segment.magnitude;

                if (cumulativeLength + segmentLength >= length)
                {
                    return segment.normalized;
                }

                cumulativeLength += segmentLength;
            }

            if (closedLoop)
            {
                return (waypoints[0].position - waypoints[waypoints.Count - 1].position).normalized;
            }

            return (waypoints[waypoints.Count - 1].position - waypoints[waypoints.Count - 2].position).normalized;
        }

        private static Vector3 CalculateDirectionAlongPath(IReadOnlyList<Vector3> pts, float length, bool closedLoop)
        {
            float cumulativeLength = 0;

            for (var i = 0; i < pts.Count - 1; i++)
            {
                var segment = pts[i + 1] - pts[i];

                if (cumulativeLength + segment.magnitude >= length)
                {
                    return segment.normalized;
                }

                cumulativeLength += segment.magnitude;
            }

            return closedLoop
                ? (pts[0] - pts[pts.Count - 1]).normalized
                : (pts[pts.Count - 1] - pts[pts.Count - 2]).normalized;
        }
    }
}