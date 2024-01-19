using UnityEngine;
using CrowdSample.Scripts.Utils;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    [RequireComponent(typeof(Path))]
    [ExecuteInEditMode]
    public class PathGizmos : MonoBehaviour
    {
        [SerializeField] private Path path;

        public Path Path => path;

        public void SetPath(Path value) => path = value;

        public Vector2 arrowScale = new Vector2(2f, 2f);

#if UNITY_EDITOR
        private void Awake()
        {
            if (Path == null) SetPath(GetComponent<Path>());
        }

        private void OnDrawGizmos()
        {
            DrawGizmos();
        }
#endif

        private void DrawGizmos()
        {
            if (Path == null) return;
            if (Path.Waypoints == null || Path.Waypoints.Count < 2) return;

            for (var i = 0; i < Path.AgentSpawnData.Length; i++)
            {
                var position  = Path.AgentSpawnData[i].position;
                var direction = Path.AgentSpawnData[i].direction;
                GizmosUtils.ThicknessArrow(position, direction, Color.yellow, arrowScale);
            }

            var waypoints     = Path.Waypoints;
            var waypointCount = waypoints.Count;

            // 決定是否畫出封閉的路徑
            var actualWaypointsCount = Path.IsClosedPath ? waypointCount : waypointCount - 1;

            for (var i = 0; i < actualWaypointsCount; i++)
            {
                var nextIndex = (i + 1) % waypointCount;
                Gizmos.DrawLine(waypoints[i].position, waypoints[nextIndex].position);
                GizmosUtils.Arrow(waypoints[i].position, waypoints[nextIndex].position - waypoints[i].position,
                    Color.cyan);
            }
        }
    }
}