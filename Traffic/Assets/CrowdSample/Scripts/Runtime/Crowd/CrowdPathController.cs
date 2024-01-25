using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using CrowdSample.Scripts.Utils;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class CrowdPathController : MonoBehaviour, IUpdateReceiver
    {
        #region Field Declarations

        [SerializeField] private List<Vector3>    pointsSet;
        [SerializeField] private List<float>      radiusSet;
        [SerializeField] private AgentSpawnData[] agentSpawnData;
        [SerializeField] private int              count    = 10;
        [SerializeField] private int              countMax = 60;
        [SerializeField] private float            spacing  = 1.0f;
        [SerializeField] private float            offset;
        [SerializeField] private bool             isSpawnAgentOnce;
        [SerializeField] private bool             isClosedPath;
        [SerializeField] private bool             isUseSpacing = true;

        #endregion

        #region Properties

        public List<Vector3>    PointsSet      => pointsSet ??= new List<Vector3>();
        public List<float>      RadiusSet      => radiusSet ??= new List<float>();
        public AgentSpawnData[] AgentSpawnData => agentSpawnData ??= Array.Empty<AgentSpawnData>();

        public int Count
        {
            get => Mathf.Clamp(count, 0, countMax);
            set => count = Mathf.Clamp(value, 0, countMax);
        }

        public int CountMax
        {
            get => countMax;
            set => countMax = value;
        }

        public float Spacing
        {
            get => Mathf.Max(spacing, 0.1f);
            set => spacing = Mathf.Max(value, 0.1f);
        }

        public float Offset
        {
            get => Mathf.Max(offset, 0f);
            set => offset = Mathf.Max(value, 0f);
        }

        public bool IsSpawnAgentOnce
        {
            get => isSpawnAgentOnce;
            set => isSpawnAgentOnce = value;
        }

        public bool IsClosedPath
        {
            get => isClosedPath;
            set => isClosedPath = value;
        }

        public bool IsUseSpacing
        {
            get => isUseSpacing;
            set => isUseSpacing = value;
        }

        public float   GetTotalLength()        => PathResolver.GetTotalLength(PointsSet, IsClosedPath);
        public Vector3 GetPositionAt(float  u) => PathResolver.GetPositionAt(PointsSet, IsClosedPath, u);
        public Vector3 GetDirectionAt(float u) => PathResolver.GetDirectionAt(PointsSet, IsClosedPath, u);

        #endregion

        #region Unity Methods

#if UNITY_EDITOR
        private void OnValidate()
        {
            FetchAllNeeded();
        }
#endif

        #endregion

        #region Public Methods

        public Vector3 GetRotationAt(float t)
        {
            var direction = GetDirectionAt(t);
            return Quaternion.LookRotation(direction).eulerAngles;
        }

#if UNITY_EDITOR
        public void FetchAllNeeded()
        {
            FetchWaypoints();
            UpdatePath();
        }
#endif

        #endregion

        #region Private Methods

        private void FetchWaypoints()
        {
            pointsSet = transform.GetComponentsInChildren<Waypoint>().Select(wp => wp.transform.position).ToList();
            radiusSet = transform.GetComponentsInChildren<Waypoint>().Select(wp => wp.Radius).ToList();
        }

        // private void FetchGenerationConfigs()
        // {
        //     if (CrowdGenerationConfig == null) return;
        //     ApplyGenerationConfigSettings();
        // }
        //
        // private void ApplyGenerationConfigSettings()
        // {
        //     isSpawnAgentOnce = CrowdGenerationConfig.IsSpawnAgentOnce;
        //     isClosedPath     = CrowdGenerationConfig.IsClosedPath;
        //     isUseSpacing     = CrowdGenerationConfig.IsUseSpacing;
        //     count            = CrowdGenerationConfig.InstantCount;
        //     countMax         = CrowdGenerationConfig.MaxCount;
        //     offset           = CrowdGenerationConfig.Offset;
        //     spacing          = CrowdGenerationConfig.Spacing;
        // }

        private void UpdatePath()
        {
            if (PointsSet.Count < 2) return;

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
            var curveU    = distance / totalLength;
            var position  = GetPositionAt(curveU);
            var direction = GetDirectionAt(curveU);
            var curvePos  = curveU * PointsSet.Count;

            agentSpawnData[index] = new AgentSpawnData(position, direction, curvePos);
        }

        #endregion

        #region Implementation Methods

        public void UpdateImmediately()
        {
            FetchAllNeeded();
        }

        #endregion
    }
}