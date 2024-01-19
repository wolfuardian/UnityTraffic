using UnityEngine;
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

        public Path                  Path                  => path;
        public AgentGenerationConfig AgentGenerationConfig => agentGenerationConfig;

        public AgentSpawnDB[] AgentSpawnData { get; private set; }

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

        public void SetPath(Path       value) => path = value;
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
                    var position  = Path.GetPositionAt(curveNPos);
                    var direction = Path.GetDirectionAt(curveNPos);
                    var curvePos  = curveNPos * Path.Waypoints.Count;
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
                    var position  = Path.GetPositionAt(curveNPos);
                    var direction = Path.GetDirectionAt(curveNPos);
                    var curvePos  = curveNPos * Path.Waypoints.Count;
                    AgentSpawnData[i] = new AgentSpawnDB(position, direction, curvePos);
                }
            }
        }
    }
}