using System.Collections.Generic;
using CrowdSample.Scripts.Runtime.Data;
using UnityEngine;
using CrowdSample.Scripts.Utils;

namespace CrowdSample.Scripts.Runtime.Agent
{
    [RequireComponent(typeof(Path))]
    [ExecuteInEditMode]
    public class PathController : MonoBehaviour, IUpdatableGizmo
    {
        [SerializeField] private Path                  path;
        [SerializeField] private AgentGenerationConfig agentGenerationConfig;

        public Path                  Path                  => path;
        public AgentGenerationConfig AgentGenerationConfig => agentGenerationConfig;

        public List<Transform> Waypoints      { get; private set; }
        public AgentSpawnDB[]  AgentSpawnData { get; private set; }

        public void SetWaypoints(List<Transform>     value) => Waypoints = value;
        public void SetAgentSpawnData(AgentSpawnDB[] value) => AgentSpawnData = value;


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
            if (path == null) path = GetComponent<Path>();
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
                SetWaypoints(new List<Transform>(waypointComponents.Length));
                for (var i = 0; i < waypointComponents.Length; i++)
                {
                    Waypoints[i] = waypointComponents[i].transform;
                }
            }
            else
            {
                Waypoints = null;
            }
        }

        public void UpdatePath()
        {
            if (path == null) return;
            if (Waypoints == null || Waypoints.Count < 2) return;

            path.SetWaypoints(new Vector3[Waypoints.Count]);
            path.SetClosedPath(ClosedPath);
            for (var i = 0; i < Waypoints.Count; i++)
            {
                if (Waypoints[i] == null) continue;
                path.Waypoints[i] = Waypoints[i].position;
            }

            SetCount(Mathf.Clamp(Count,     0,    CountMax));
            SetSpacing(Mathf.Clamp(Spacing, 0.1f, float.MaxValue));
            SetOffset(Mathf.Clamp(Offset,   0f,   float.MaxValue));

            CalculatePositionsAndDirections();
        }

        private void CalculatePositionsAndDirections()
        {
            var totalLength = path.GetTotalLength();
            var maxCount    = Mathf.FloorToInt(totalLength / Spacing); // 根據總長度和間距計算最大數量

            SetAgentSpawnData(new AgentSpawnDB[Count]);

            if (UseSpacing)
            {
                SetCount(Mathf.Min(Count, maxCount)); // 限制 count 不超過最大數量

                for (var i = 0; i < Count; i++)
                {
                    float distance;
                    if (ClosedPath)
                    {
                        distance = (Offset + Spacing * i) % totalLength; // 循環回路徑開始
                    }
                    else
                    {
                        distance = Offset + Spacing * i;
                        if (distance > totalLength) break; // 如果不是封閉的，超出路徑就停止
                    }

                    var curveNPos = distance / totalLength;
                    var position  = path.GetPositionAt(curveNPos);
                    var direction = path.GetDirectionAt(curveNPos);
                    var curvePos  = curveNPos * Waypoints.Count;
                    AgentSpawnData[i] = new AgentSpawnDB(position, direction, curvePos);
                }
            }
            else
            {
                for (var i = 0; i < Count; i++)
                {
                    var curveNPos = (float)i / Count;
                    curveNPos += Offset / totalLength;
                    curveNPos %= 1.0f;
                    var position  = path.GetPositionAt(curveNPos);
                    var direction = path.GetDirectionAt(curveNPos);
                    var curvePos  = curveNPos * Waypoints.Count;
                    AgentSpawnData[i] = new AgentSpawnDB(position, direction, curvePos);
                }
            }
        }
    }
}