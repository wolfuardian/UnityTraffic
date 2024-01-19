﻿using UnityEngine;
using CrowdSample.Scripts.Utils;
using System.Collections.Generic;
using CrowdSample.Scripts.Runtime.Data;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    [RequireComponent(typeof(Path))]
    [ExecuteInEditMode]
    public class PathController : MonoBehaviour, IUpdatableGizmo
    {
        [SerializeField] private Path                  path;
        [SerializeField] private AgentGenerationConfig agentGenerationConfig;
        [SerializeField] private AgentSpawnDB[]        agentSpawnDB;

        public Path                  Path                  => path;
        public AgentGenerationConfig AgentGenerationConfig => agentGenerationConfig;
        public AgentSpawnDB[]        AgentSpawnDB          => agentSpawnDB;


        public void SetPath(Path                   value) => path = value;
        public void SetAgentSpawnDB(AgentSpawnDB[] value) => agentSpawnDB = value;


        #region Parameter Variables

        [SerializeField] private bool  closedPath;
        [SerializeField] private int   count    = 10;
        [SerializeField] private int   countMax = 60;
        [SerializeField] private float spacing  = 1.0f;
        [SerializeField] private float offset;
        [SerializeField] private bool  useSpacing = true;

        public bool  ClosedPath => closedPath;
        public int   Count      => count;
        public int   CountMax   => countMax;
        public float Spacing    => spacing;
        public float Offset     => offset;
        public bool  UseSpacing => useSpacing;

        public void SetClosedPath(bool value) => closedPath = value;
        public void SetCount(int       value) => count = value;
        public void SetCountMax(int    value) => countMax = value;
        public void SetOffset(float    value) => offset = value;
        public void SetUseSpacing(bool value) => useSpacing = value;
        public void SetSpacing(float   value) => spacing = value;

        #endregion


#if UNITY_EDITOR
        private void Awake()
        {
            if (Path == null) SetPath(GetComponent<Path>());
        }

        private void OnValidate()
        {
            FetchGenerationConfigs();
            UpdateWaypoints();
            UpdatePath();
        }


        public void UpdateGizmo()
        {
            FetchGenerationConfigs();
            UpdatePath();
        }
#endif
        private void FetchGenerationConfigs()
        {
            if (AgentGenerationConfig == null) return;
            SetClosedPath(AgentGenerationConfig.ClosedPath);
            SetCount(AgentGenerationConfig.InstantCount);
            SetCountMax(AgentGenerationConfig.MaxCount);
            SetOffset(AgentGenerationConfig.Offset);
            SetUseSpacing(AgentGenerationConfig.UseSpacing);
            SetSpacing(AgentGenerationConfig.Spacing);
        }

        private void UpdateWaypoints()
        {
            var waypointComponents = transform.GetComponentsInChildren<Waypoint>();
            if (waypointComponents.Length > 0)
            {
                Path.SetWaypoints(new List<Transform>(waypointComponents.Length));
                for (var i = 0; i < waypointComponents.Length; i++)
                {
                    Path.Waypoints[i] = waypointComponents[i].transform;
                }
            }
            else
            {
                Path.SetWaypoints(new List<Transform>());
            }
        }

        public void UpdatePath()
        {
            if (Path == null) return;
            if (Path.Waypoints == null || Path.Waypoints.Count < 2) return;

            SetCount(Mathf.Clamp(Count,     0,    CountMax));
            SetSpacing(Mathf.Clamp(Spacing, 0.1f, float.MaxValue));
            SetOffset(Mathf.Clamp(Offset,   0f,   float.MaxValue));

            CalculatePositionsAndDirections();
        }

        private void CalculatePositionsAndDirections()
        {
            var totalLength = Path.GetTotalLength();
            var maxCount    = Mathf.FloorToInt(totalLength / Spacing);

            SetAgentSpawnDB(new AgentSpawnDB[Count]);

            if (UseSpacing)
            {
                SetCount(Mathf.Min(Count, maxCount));
            }

            for (var i = 0; i < Count; i++)
            {
                var distance = CalculateDistance(i, totalLength);
                if (distance > totalLength && !ClosedPath) break;

                SetAgentData(i, distance, totalLength);
            }
        }

        private float CalculateDistance(int index, float totalLength)
        {
            if (UseSpacing)
            {
                if (ClosedPath)
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
            var position  = Path.GetPositionAt(curveNPos);
            var direction = Path.GetDirectionAt(curveNPos);
            var curvePos  = curveNPos * Path.Waypoints.Count;
            AgentSpawnDB[index] = new AgentSpawnDB(position, direction, curvePos);
        }
    }
}