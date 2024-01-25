using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using CrowdSample.Scripts.Runtime.Data;
using CrowdSample.Scripts.Utils;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class CrowdPathController : MonoBehaviour, IUpdateReceiver
    {
        #region Field Declarations

        [SerializeField] private List<Transform>  waypoints;
        [SerializeField] private AgentSpawnData[] agentSpawnData;
        [SerializeField] private bool             isSpawnAgentOnce;
        [SerializeField] private bool             isClosedPath;
        [SerializeField] private bool             isUseSpacing = true;
        [SerializeField] private int              count        = 10;
        [SerializeField] private int              countMax     = 60;
        [SerializeField] private float            spacing      = 1.0f;
        [SerializeField] private float            offset;

        /// <summary>
        /// 路徑中的航點。
        /// </summary>
        public List<Transform> Waypoints => waypoints ??= new List<Transform>();

        /// <summary>
        /// 代理生成配置。
        /// </summary>
        /// <summary>
        /// 代理生成數據。
        /// </summary>
        public AgentSpawnData[] AgentSpawnData => agentSpawnData ??= Array.Empty<AgentSpawnData>();

        /// <summary>
        /// 是否只生成一次代理。
        /// </summary>
        public bool IsSpawnAgentOnce => isSpawnAgentOnce;

        /// <summary>
        /// 路徑是否閉合。
        /// </summary>
        public bool IsClosedPath => isClosedPath;

        /// <summary>
        /// 是否使用間隔。
        /// </summary>
        public bool IsUseSpacing => isUseSpacing;

        /// <summary>
        /// 代理的數量，限制在 0 到 CountMax 之間。
        /// </summary>
        public int Count
        {
            get => Mathf.Clamp(count, 0, countMax);
            set => count = Mathf.Clamp(value, 0, countMax);
        }

        /// <summary>
        /// 代理數量的最大值。
        /// </summary>
        public int CountMax => countMax;

        /// <summary>
        /// 代理間的間隔，最小為 0.1。
        /// </summary>
        public float Spacing
        {
            get => Mathf.Max(spacing, 0.1f);
            set => spacing = Mathf.Max(value, 0.1f);
        }

        /// <summary>
        /// 偏移量，最小為 0。
        /// </summary>
        public float Offset
        {
            get => Mathf.Max(offset, 0f);
            set => offset = Mathf.Max(value, 0f);
        }

        #endregion

        public float   GetTotalLength()        => PathResolver.GetTotalLength(Waypoints, IsClosedPath);
        public Vector3 GetPositionAt(float  t) => PathResolver.GetPositionAt(Waypoints, IsClosedPath, t);
        public Vector3 GetDirectionAt(float t) => PathResolver.GetDirectionAt(Waypoints, IsClosedPath, t);

        public Vector3 GetRotationAt(float t)
        {
            var direction = GetDirectionAt(t);
            return Quaternion.LookRotation(direction).eulerAngles;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdatePathConfiguration();
        }

        public void UpdateImmediately()
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
            // if (CrowdGenerationConfig == null) return;
            // ApplyGenerationConfigSettings();
        }

        private void ApplyGenerationConfigSettings()
        {
            // isSpawnAgentOnce = CrowdGenerationConfig.IsSpawnAgentOnce;
            // isClosedPath     = CrowdGenerationConfig.IsClosedPath;
            // isUseSpacing     = CrowdGenerationConfig.IsUseSpacing;
            // count            = CrowdGenerationConfig.InstantCount;
            // countMax         = CrowdGenerationConfig.MaxCount;
            // offset           = CrowdGenerationConfig.Offset;
            // spacing          = CrowdGenerationConfig.Spacing;
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

            var actualCount = 0;


            if (IsUseSpacing)
            {
                count = Mathf.Min(Count, maxCount);
            }

            for (var i = 0; i < Count; i++)
            {
                var distance = CalculateDistance(i, totalLength);
                if (distance > totalLength && !IsClosedPath) break;
                actualCount++;
            }

            agentSpawnData = new AgentSpawnData[actualCount];

            for (var i = 0; i < actualCount; i++)
            {
                var distance = CalculateDistance(i, totalLength);
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
            var distance = Offset + Spacing * index;
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