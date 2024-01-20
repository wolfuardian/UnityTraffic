using UnityEngine;
using CrowdSample.Scripts.Utils;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    [RequireComponent(typeof(Path))]
    [ExecuteInEditMode]
    public class PathGizmos : MonoBehaviour
    {
        [SerializeField] private Path path;

        public Path Path => path ??= GetComponent<Path>();

        public Vector2 arrowScale = new Vector2(2f, 2f);

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            DrawGizmos();
        }
#endif

        public void DrawGizmos()
        {
            if (!ValidatePath()) return;

            DrawAgentSpawnPoints();
            DrawPathWaypoints();
        }

        private void DrawAgentSpawnPoints()
        {
            foreach (var spawnData in Path.AgentSpawnData)
            {
                GizmosUtils.ThicknessArrow(spawnData.position, spawnData.direction, Color.yellow, arrowScale);
            }
        }

        private void DrawPathWaypoints()
        {
            var waypoints            = Path.Waypoints;
            var waypointCount        = waypoints.Count;
            var actualWaypointsCount = Path.IsClosedPath ? waypointCount : waypointCount - 1;

            for (var i = 0; i < actualWaypointsCount; i++)
            {
                var nextIndex = (i + 1) % waypointCount;
                if (waypoints[i] == null || waypoints[nextIndex] == null) continue;

                Gizmos.DrawLine(waypoints[i].position, waypoints[nextIndex].position);
                GizmosUtils.Arrow(waypoints[i].position, waypoints[nextIndex].position - waypoints[i].position,
                    Color.cyan);
            }
        }

        private bool ValidatePath()
        {
            if (Path.Waypoints.Count < 2)
            {
                Debug.LogWarning("路徑物件的Waypoints數量不足（至少需要2個）。");
                return false;
            }

            return true;
        }
    }
}