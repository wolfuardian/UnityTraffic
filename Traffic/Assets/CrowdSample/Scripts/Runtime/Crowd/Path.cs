using UnityEngine;
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

        public List<Transform>       Waypoints             => waypoints;
        public AgentGenerationConfig AgentGenerationConfig => agentGenerationConfig;
        public AgentSpawnData[]      AgentSpawnData        => agentSpawnData;

        public void SetWaypoints(List<Transform>       value) => waypoints = value;
        public void SetAgentSpawnData(AgentSpawnData[] value) => agentSpawnData = value;

        #region Parameter Variables

        [SerializeField] private bool  isSpawnAgentOnce;
        [SerializeField] private bool  isClosedPath;
        [SerializeField] private bool  isUseSpacing = true;
        [SerializeField] private int   count        = 10;
        [SerializeField] private int   countMax     = 60;
        [SerializeField] private float spacing      = 1.0f;
        [SerializeField] private float offset;

        public bool  IsSpawnAgentOnce => isSpawnAgentOnce;
        public bool  IsClosedPath     => isClosedPath;
        public bool  IsUseSpacing     => isUseSpacing;
        public int   Count            => count;
        public int   CountMax         => countMax;
        public float Spacing          => spacing;
        public float Offset           => offset;

        public void SetIsSpawnAgentOnce(bool value) => isSpawnAgentOnce = value;
        public void SetIsClosedPath(bool     value) => isClosedPath = value;
        public void SetIsUseSpacing(bool     value) => isUseSpacing = value;
        public void SetCount(int             value) => count = value;
        public void SetCountMax(int          value) => countMax = value;
        public void SetOffset(float          value) => offset = value;
        public void SetSpacing(float         value) => spacing = value;

        #endregion

        public float   GetTotalLength()        => PathResolver.GetTotalLength(Waypoints, IsClosedPath);
        public Vector3 GetPositionAt(float  t) => PathResolver.GetPositionAt(Waypoints, IsClosedPath, t);
        public Vector3 GetDirectionAt(float t) => PathResolver.GetDirectionAt(Waypoints, IsClosedPath, t);

#if UNITY_EDITOR
        private void OnValidate()
        {
            FetchWaypoints();
            FetchGenerationConfigs();
        }

        public void UpdateGizmo()
        {
            FetchGenerationConfigs();
            UpdatePath();
        }
#endif
        private void FetchWaypoints()
        {
            SetWaypoints(new List<Transform>());

            var children = transform.GetComponentsInChildren<Waypoint>();
            if (children.Length <= 0) return;

            foreach (var child in children)
            {
                Waypoints.Add(child.transform);
            }
        }

        private void FetchGenerationConfigs()
        {
            if (AgentGenerationConfig == null) return;
            SetIsSpawnAgentOnce(AgentGenerationConfig.IsSpawnAgentOnce);
            SetIsClosedPath(AgentGenerationConfig.IsClosedPath);
            SetIsUseSpacing(AgentGenerationConfig.IsUseSpacing);
            SetCount(AgentGenerationConfig.InstantCount);
            SetCountMax(AgentGenerationConfig.MaxCount);
            SetOffset(AgentGenerationConfig.Offset);
            SetSpacing(AgentGenerationConfig.Spacing);
        }

        private void UpdatePath()
        {
            if (Waypoints == null || Waypoints.Count < 2) return;

            SetCount(Mathf.Clamp(Count,     0,    CountMax));
            SetSpacing(Mathf.Clamp(Spacing, 0.1f, float.MaxValue));
            SetOffset(Mathf.Clamp(Offset,   0f,   float.MaxValue));

            CalculatePositionsAndDirections();
        }

        private void CalculatePositionsAndDirections()
        {
            var totalLength = GetTotalLength();
            var maxCount    = Mathf.FloorToInt(totalLength / Spacing);

            SetAgentSpawnData(new AgentSpawnData[Count]);

            if (IsUseSpacing)
            {
                SetCount(Mathf.Min(Count, maxCount));
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
            if (IsUseSpacing)
            {
                if (IsClosedPath)
                {
                    return (Offset + Spacing * index) % totalLength;
                }

                return Offset + Spacing * index;
            }

            var curveNPos = (float)index / Count;
            return (curveNPos + Offset / totalLength) % 1.0f * totalLength;
        }

        private void SetAgentData(int index, float distance, float totalLength)
        {
            var curveNPos = distance / totalLength;
            var position  = GetPositionAt(curveNPos);
            var direction = GetDirectionAt(curveNPos);
            var curvePos  = curveNPos * Waypoints.Count;
            AgentSpawnData[index] = new AgentSpawnData(position, direction, curvePos);
        }
    }
}