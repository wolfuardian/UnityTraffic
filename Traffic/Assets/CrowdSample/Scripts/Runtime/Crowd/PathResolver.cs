using UnityEngine;
using System.Collections.Generic;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public static class PathResolver
    {
        public static AgentSpawnDB[] CalculatePositionsAndDirections(
            Path  path,
            bool  closedPath,
            int   count,
            float spacing,
            float offset,
            bool  useSpacing)
        {
            var agentSpawnDB = new AgentSpawnDB[count];
            var totalLength  = path.GetTotalLength();
            var maxCount     = Mathf.FloorToInt(totalLength / spacing);

            for (var i = 0; i < count; i++)
            {
                float curveNPos;
                if (useSpacing)
                {
                    count = Mathf.Min(count, maxCount);
                    var distance = closedPath ? (offset + spacing * i) % totalLength : offset + spacing * i;
                    if (!closedPath && distance > totalLength) break;
                    curveNPos = distance / totalLength;
                }
                else
                {
                    curveNPos = (float)i / count + offset / totalLength;
                    if (closedPath) curveNPos %= 1.0f;
                }

                var position  = path.GetPositionAt(curveNPos);
                var direction = path.GetDirectionAt(curveNPos);
                var curvePos  = curveNPos * path.Waypoints.Count;
                agentSpawnDB[i] = new AgentSpawnDB(position, direction, curvePos);
            }

            return agentSpawnDB;
        }


        public static float GetTotalLength(List<Transform> waypoints, bool isClosedPath)
        {
            if (waypoints == null || waypoints.Count < 2)
            {
                Debug.LogError("Insufficient checkpoints to form a path.");
                return 0;
            }

            float length = 0;
            for (var i = 0; i < waypoints.Count - 1; i++)
            {
                length += Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);
            }

            if (isClosedPath && waypoints.Count > 2)
            {
                length += Vector3.Distance(waypoints[waypoints.Count - 1].position, waypoints[0].position);
            }

            return length;
        }

        public static Vector3 GetPositionAt(List<Transform> waypoints, bool isClosedPath, float t)
        {
            if (waypoints == null || waypoints.Count < 2)
            {
                Debug.LogError("Path waypoints are not set or insufficient.");
                return Vector3.zero;
            }

            if (t <= 0) return waypoints[0].position;
            if (t >= 1) return waypoints[waypoints.Count - 1].position;

            var   totalLength      = GetTotalLength(waypoints, isClosedPath);
            var   targetLength     = t * totalLength;
            float cumulativeLength = 0;

            for (var i = 0; i < waypoints.Count - 1; i++)
            {
                var segmentLength = Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);

                if (cumulativeLength + segmentLength >= targetLength)
                {
                    var remainingLength = targetLength - cumulativeLength;
                    var segment         = waypoints[i + 1].position - waypoints[i].position;
                    return waypoints[i].position + segment.normalized * remainingLength;
                }

                cumulativeLength += segmentLength;
            }

            if (isClosedPath)
            {
                var finalSegment = waypoints[0].position - waypoints[waypoints.Count - 1].position;
                return waypoints[waypoints.Count - 1].position +
                       finalSegment.normalized * (targetLength - cumulativeLength);
            }

            return waypoints[waypoints.Count - 1].position;
        }

        public static Vector3 GetDirectionAt(List<Transform> waypoints, bool isClosedPath, float t)
        {
            if (waypoints == null || waypoints.Count < 2)
            {
                Debug.LogError("Path waypoints are not set or insufficient.");
                return Vector3.zero;
            }

            if (t <= 0) return (waypoints[1].position - waypoints[0].position).normalized;
            if (t >= 1) return (waypoints[0].position - waypoints[waypoints.Count - 1].position).normalized;

            var   totalLength      = GetTotalLength(waypoints, isClosedPath);
            var   targetLength     = t * totalLength;
            float cumulativeLength = 0;

            for (var i = 0; i < waypoints.Count - 1; i++)
            {
                var segmentLength = Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);

                if (cumulativeLength + segmentLength >= targetLength)
                {
                    return (waypoints[i + 1].position - waypoints[i].position).normalized;
                }

                cumulativeLength += segmentLength;
            }

            if (isClosedPath)
            {
                return (waypoints[0].position - waypoints[waypoints.Count - 1].position).normalized;
            }

            return (waypoints[waypoints.Count - 1].position - waypoints[waypoints.Count - 2].position).normalized;
        }
    }
}