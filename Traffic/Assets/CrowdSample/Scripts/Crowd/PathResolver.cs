﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CrowdSample.Scripts.Crowd
{
    public static class PathResolver
    {
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

        public static float CalculateDistance(CrowdConfig config, int index, float totalLength)
        {
            return config.useSpacing
                ? CalculateSpacingDistance(config, index, totalLength)
                : CalculateCurveDistance(config, index, totalLength);
        }

        public static float CalculateSpacingDistance(CrowdConfig config, int index, float totalLength)
        {
            var distance = config.offset + config.spacing * index;
            return config.pathClosed ? distance % totalLength : distance;
        }

        public static float CalculateCurveDistance(CrowdConfig config, int index, float totalLength)
        {
            var interp = (float)index / config.instantCount;
            return (interp + config.offset / totalLength) % 1.0f * totalLength;
        }

        public static Vector3 GetPositionAt(CrowdConfig config, List<Vector3> pts, float interp)
        {
            if (!ValidateWaypoints(pts)) return Vector3.zero;

            if (interp <= 0) return pts[0];
            if (interp >= 1) return pts[pts.Count - 1];

            var totalLength = GetTotalLength(pts, config.pathClosed);
            var length      = interp * totalLength;
            return CalculatePositionAlongPath(pts, length, config.pathClosed);
        }

        public static Vector3 GetDirectionAt(CrowdConfig config, List<Vector3> pts, float interp)
        {
            if (!ValidateWaypoints(pts)) return Vector3.zero;

            if (interp <= 0) return (pts[1] - pts[0]).normalized;
            if (interp >= 1) return (pts[pts.Count - 1] - pts[0]).normalized;

            var totalLength = GetTotalLength(pts, config.pathClosed);
            var length      = interp * totalLength;
            return CalculateDirectionAlongPath(pts, length, config.pathClosed);
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

        private static float CalculatePathLength(IReadOnlyList<Vector3> pts)
        {
            float length = 0;
            for (var i = 0; i < pts.Count - 1; i++)
            {
                length += Vector3.Distance(pts[i], pts[i + 1]);
            }

            return length;
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

        public static float GetLocalDistanceAt(CrowdConfig config, List<Vector3> pts, float interp)
        {
            if (!ValidateWaypoints(pts)) return 0;

            var totalLength  = GetTotalLength(pts, config.pathClosed);
            var globalLength = interp * totalLength;
            var (currentIndex, localDistance) = CalculateLocalDistance(pts, globalLength, config.pathClosed);
            return currentIndex + localDistance;
        }

        private static (int, float) CalculateLocalDistance(IReadOnlyList<Vector3> pts, float globalLength,
            bool                                                                  closedLoop)
        {
            float cumulativeLength = 0;

            for (var i = 0; i < pts.Count - 1; i++)
            {
                var segment              = pts[i + 1] - pts[i];
                var segmentLength        = segment.magnitude;
                var nextCumulativeLength = cumulativeLength + segmentLength;

                if (nextCumulativeLength >= globalLength)
                {
                    var localLength = globalLength - cumulativeLength;
                    return (i, localLength / segmentLength);
                }

                cumulativeLength = nextCumulativeLength;
            }

            if (closedLoop)
            {
                var finalSegment       = pts[0] - pts[pts.Count - 1];
                var finalSegmentLength = finalSegment.magnitude;
                var finalLength        = cumulativeLength + finalSegmentLength;
                if (finalLength >= globalLength)
                {
                    var localLength = globalLength - cumulativeLength;
                    return (pts.Count - 1, localLength / finalSegmentLength);
                }
            }

            return (pts.Count - 2, 1.0f);
        }
    }
}