using UnityEngine;
using CrowdSample.Scripts.Utils;

namespace CrowdSample.Scripts.Runtime.Agent
{
    [RequireComponent(typeof(Path))]
    [ExecuteInEditMode]
    public class PathController : MonoBehaviour, IUpdatableGizmo
    {
        public Path path;

        [SerializeField] private int         count    = 10;
        [SerializeField] private int         countMax = 60;
        [SerializeField] private float       spacing  = 1.0f;
        [SerializeField] private float       offset;
        [SerializeField] private bool        useSpacing;
        [SerializeField] private Transform[] waypointTransforms;
        [SerializeField] private Vector3[]   positions;
        [SerializeField] private Vector3[]   directions;
        [SerializeField] private float[]     segments;


        public Transform[] WaypointTransforms => waypointTransforms;
        public int         Count              => count;
        public int         CountMax           => countMax;
        public float       Spacing            => spacing;
        public float       Offset             => offset;
        public bool        UseSpacing         => useSpacing;

        public Vector3[] Positions  => positions;
        public Vector3[] Directions => directions;
        public float[]   Segments   => segments;

        public void SetCount(int            value) => count = value;
        public void SetSpacing(float        value) => spacing = value;
        public void SetOffset(float         value) => offset = value;
        public void SetPositions(Vector3[]  value) => positions = value;
        public void SetDirections(Vector3[] value) => directions = value;
        public void SetCurveus(float[]      value) => segments = value;

#if UNITY_EDITOR
        private void Awake()
        {
            if (path == null) path = GetComponent<Path>();
        }

        private void OnValidate()
        {
            UpdatePath();
        }

        public void UpdateGizmo()
        {
            UpdatePath();
        }
#endif

        public void UpdatePath()
        {
            if (path == null) return;
            if (WaypointTransforms == null || WaypointTransforms.Length < 2) return;

            path.SetWaypoints(new Vector3[WaypointTransforms.Length]);
            for (var i = 0; i < WaypointTransforms.Length; i++)
            {
                if (WaypointTransforms[i] == null) continue;
                path.SetWaypointsElem(i, WaypointTransforms[i].position);
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

            SetPositions(new Vector3[Count]);
            SetDirections(new Vector3[Count]);
            SetCurveus(new float[Count]);

            if (UseSpacing)
            {
                SetCount(Mathf.Min(Count, maxCount)); // 限制 count 不超過最大數量

                for (var i = 0; i < count; i++)
                {
                    float distance;
                    if (path.isClosed)
                    {
                        distance = (Offset + spacing * i) % totalLength; // 循環回路徑開始
                    }
                    else
                    {
                        distance = Offset + spacing * i;
                        if (distance > totalLength) break; // 如果不是封閉的，超出路徑就停止
                    }

                    var curveu = distance / totalLength;
                    Positions[i]  = path.GetPointAt(curveu);
                    Directions[i] = path.GetDirectionAt(curveu);
                    Segments[i]   = curveu * WaypointTransforms.Length;
                }
            }
            else
            {
                for (var i = 0; i < Count; i++)
                {
                    var curveu = (float)i / Count;
                    curveu        += Offset / totalLength;
                    curveu        %= 1.0f;
                    Positions[i]  =  path.GetPointAt(curveu);
                    Directions[i] =  path.GetDirectionAt(curveu);
                    Segments[i]   =  curveu * WaypointTransforms.Length;
                }
            }
        }
    }
}