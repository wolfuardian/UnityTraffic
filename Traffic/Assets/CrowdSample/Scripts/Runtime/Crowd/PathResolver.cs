using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public static class PathResolver
    {
        public static float GetTotalLength(List<Transform> pts, bool closedLoop)
        {
            if (!ValidateWaypoints(pts)) return 0;

            var length = CalculatePathLength(pts);

            if (closedLoop)
            {
                length += Vector3.Distance(pts[pts.Count - 1].position, pts[0].position);
            }

            return length;
        }

        public static float GetTotalLength(List<Vector3> pts, bool closedLoop)
        {
            if (!ValidateWaypoints(pts)) return 0;

            var length = CalculatePathLength(pts);

            if (closedLoop)
            {
                length += Vector3.Distance(pts[pts.Count - 1], pts[0]);
            }

            return length;
        }

        public static Vector3 GetPositionAt(List<Transform> pts, bool closedLoop, float curveU)
        {
            if (!ValidateWaypoints(pts)) return Vector3.zero;

            if (curveU <= 0) return pts[0].position;
            if (curveU >= 1) return pts[pts.Count - 1].position;

            var totalLength = GetTotalLength(pts, closedLoop);
            var length      = curveU * totalLength;
            return CalculatePositionAlongPath(pts, length, closedLoop);
        }

        public static Vector3 GetPositionAt(List<Vector3> pts, bool closedLoop, float curveU)
        {
            if (!ValidateWaypoints(pts)) return Vector3.zero;

            if (curveU <= 0) return pts[0];
            if (curveU >= 1) return pts[pts.Count - 1];

            var totalLength = GetTotalLength(pts, closedLoop);
            var length      = curveU * totalLength;
            return CalculatePositionAlongPath(pts, length, closedLoop);
        }

        public static Vector3 GetDirectionAt(List<Transform> pts, bool closedLoop, float curveU)
        {
            if (!ValidateWaypoints(pts)) return Vector3.zero;

            if (curveU <= 0) return (pts[1].position - pts[0].position).normalized;
            if (curveU >= 1) return (pts[pts.Count - 1].position - pts[0].position).normalized;

            var totalLength = GetTotalLength(pts, closedLoop);
            var length      = curveU * totalLength;
            return CalculateDirectionAlongPath(pts, length, closedLoop);
        }

        public static Vector3 GetDirectionAt(List<Vector3> pts, bool closedLoop, float curveU)
        {
            if (!ValidateWaypoints(pts)) return Vector3.zero;

            if (curveU <= 0) return (pts[1] - pts[0]).normalized;
            if (curveU >= 1) return (pts[pts.Count - 1] - pts[0]).normalized;

            var totalLength = GetTotalLength(pts, closedLoop);
            var length      = curveU * totalLength;
            return CalculateDirectionAlongPath(pts, length, closedLoop);
        }


        public static Vector3 GetRotationAt(List<Transform> pts, bool closedLoop, float curveU)
        {
            if (!ValidateWaypoints(pts)) return Vector3.zero;

            if (curveU <= 0)
            {
                return Quaternion.LookRotation(pts[1].position - pts[0].position).eulerAngles;
            }

            if (curveU >= 1)
            {
                return Quaternion
                    .LookRotation(pts[pts.Count - 1].position - pts[pts.Count - 2].position)
                    .eulerAngles;
            }

            var totalLength = GetTotalLength(pts, closedLoop);
            var length      = curveU * totalLength;
            var direction   = CalculateDirectionAlongPath(pts, length, closedLoop);
            return Quaternion.LookRotation(direction).eulerAngles;
        }

        private static bool ValidateWaypoints(ICollection pts)
        {
            if (pts == null || pts.Count < 2)
            {
                Debug.LogError("Path waypoints are not set or insufficient.");
                return false;
            }

            return true;
        }

        private static bool ValidateWaypoints(List<Vector3> pts)
        {
            if (pts == null || pts.Count < 2)
            {
                Debug.LogError("Path waypoints are not set or insufficient.");
                return false;
            }

            return true;
        }

        private static float CalculatePathLength(IReadOnlyList<Transform> pts)
        {
            float length = 0;
            for (var i = 0; i < pts.Count - 1; i++)
            {
                length += Vector3.Distance(pts[i].position, pts[i + 1].position);
            }

            return length;
        }

        private static float CalculatePathLength(IReadOnlyList<Vector3> pts)
        {
            float length = 0;
            for (var i = 0; i < pts.Count - 1; i++)
            {
                length += Vector3.Distance(pts[i], pts[i + 1]);
            }

            return length;
        }

        private static Vector3 CalculatePositionAlongPath(IReadOnlyList<Transform> pts, float length, bool closedLoop)
        {
            float cumulativeLength = 0;

            for (var i = 0; i < pts.Count - 1; i++)
            {
                var segment       = pts[i + 1].position - pts[i].position;
                var segmentLength = segment.magnitude;

                if (cumulativeLength + segmentLength >= length)
                {
                    var remainingLength = length - cumulativeLength;
                    return pts[i].position + segment.normalized * remainingLength;
                }

                cumulativeLength += segmentLength;
            }

            if (closedLoop)
            {
                var finalSegment = pts[0].position - pts[pts.Count - 1].position;
                return pts[pts.Count - 1].position +
                       finalSegment.normalized * (length - cumulativeLength);
            }

            return pts[pts.Count - 1].position;
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

        private static Vector3 CalculateDirectionAlongPath(IReadOnlyList<Transform> pts, float length, bool closedLoop)
        {
            float cumulativeLength = 0;

            for (var i = 0; i < pts.Count - 1; i++)
            {
                var segment       = pts[i + 1].position - pts[i].position;
                var segmentLength = segment.magnitude;

                if (cumulativeLength + segmentLength >= length)
                {
                    return segment.normalized;
                }

                cumulativeLength += segmentLength;
            }

            if (closedLoop)
            {
                return (pts[0].position - pts[pts.Count - 1].position).normalized;
            }

            return (pts[pts.Count - 1].position - pts[pts.Count - 2].position).normalized;
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