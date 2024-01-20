using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using CrowdSample.Scripts.Runtime.Data;
using CrowdSample.Scripts.Utils;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class Path : MonoBehaviour, IUpdatable
    {
        [SerializeField] private List<Transform>       waypoints;
        [SerializeField] private AgentGenerationConfig agentGenerationConfig;
        [SerializeField] private AgentSpawnData[]      agentSpawnData;

        public List<Transform> Waypoints => waypoints ??= new List<Transform>();

        public AgentGenerationConfig       AgentGenerationConfig => agentGenerationConfig;
        public IEnumerable<AgentSpawnData> AgentSpawnData        => agentSpawnData ??= Array.Empty<AgentSpawnData>();

        #region Parameter Variables

        [SerializeField] private bool  isSpawnAgentOnce;
        [SerializeField] private bool  isClosedPath;
        [SerializeField] private bool  isUseSpacing = true;
        [SerializeField] private int   count        = 10;
        [SerializeField] private int   countMax     = 60;
        [SerializeField] private float spacing      = 1.0f;
        [SerializeField] private float offset;

        // Properties for publicly exposing fields
        public bool  IsSpawnAgentOnce => isSpawnAgentOnce;
        public bool  IsClosedPath     => isClosedPath;
        public bool  IsUseSpacing     => isUseSpacing;
        public int   Count            => Mathf.Clamp(count, 0, CountMax);
        public int   CountMax         => countMax;
        public float Spacing          => Mathf.Max(spacing, 0.1f);
        public float Offset           => Mathf.Max(offset,  0f);

        #endregion

        public float   GetTotalLength()        => PathResolver.GetTotalLength(Waypoints, IsClosedPath);
        public Vector3 GetPositionAt(float  t) => PathResolver.GetPositionAt(Waypoints, IsClosedPath, t);
        public Vector3 GetDirectionAt(float t) => PathResolver.GetDirectionAt(Waypoints, IsClosedPath, t);

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdatePathConfiguration();
        }

        public void UpdateGizmo()
        {
            UpdatePathConfiguration();
        }

        public void UpdatePathConfiguration()
        {
            FetchWaypoints();
            FetchGenerationConfigs();
            UpdatePath();
        }
#endif

        private void FetchWaypoints()
        {
            waypoints = transform.GetComponentsInChildren<Waypoint>().Select(wp => wp.transform).ToList();
        }

        private void FetchGenerationConfigs()
        {
            if (AgentGenerationConfig == null) return;
            ApplyGenerationConfigSettings();
        }

        private void ApplyGenerationConfigSettings()
        {
            isSpawnAgentOnce = AgentGenerationConfig.IsSpawnAgentOnce;
            isClosedPath     = AgentGenerationConfig.IsClosedPath;
            isUseSpacing     = AgentGenerationConfig.IsUseSpacing;
            count            = AgentGenerationConfig.InstantCount;
            countMax         = AgentGenerationConfig.MaxCount;
            offset           = AgentGenerationConfig.Offset;
            spacing          = AgentGenerationConfig.Spacing;
        }

        private void UpdatePath()
        {
            if (Waypoints.Count < 2) return;

            CalculatePositionsAndDirections();
        }

        private void CalculatePositionsAndDirections()
        {
            var totalLength = GetTotalLength();
            var maxCount    = Mathf.FloorToInt(totalLength / Spacing);

            agentSpawnData = new AgentSpawnData[Count];

            if (IsUseSpacing)
            {
                count = Mathf.Min(Count, maxCount);
            }

            for (var i = 0; i < Count; i++)
            {
                var distance = CalculateDistance(i, totalLength);
                if (distance > totalLength && !IsClosedPath) break;

                SetAgentData(i, distance, totalLength);
            }
        }

        private float CalculateDistance(int index, float totalLength)
        {
            return IsUseSpacing
                ? CalculateSpacingDistance(index, totalLength)
                : CalculateCurveDistance(index, totalLength);
        }

        private float CalculateSpacingDistance(int index, float totalLength)
        {
            float distance = Offset + Spacing * index;
            return IsClosedPath ? distance % totalLength : distance;
        }

        private float CalculateCurveDistance(int index, float totalLength)
        {
            var curveNPos = (float)index / Count;
            return (curveNPos + Offset / totalLength) % 1.0f * totalLength;
        }

        private void SetAgentData(int index, float distance, float totalLength)
        {
            var curveNPos = distance / totalLength;
            var position  = GetPositionAt(curveNPos);
            var direction = GetDirectionAt(curveNPos);
            var curvePos  = curveNPos * Waypoints.Count;

            agentSpawnData[index] = new AgentSpawnData(position, direction, curvePos);
        }
    }
}