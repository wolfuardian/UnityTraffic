using UnityEngine;
using CrowdSample.Scripts.Utils;

namespace CrowdSample.Scripts.Runtime.Agent
{
    public class PathGizmos : MonoBehaviour, IUpdatableGizmo
    {
        public PathController pathController;
        public Vector2        arrowScale = new Vector2(2f, 2f);

        private void OnDrawGizmos()
        {
            var path = pathController.path;
            if (pathController == null || path == null) return;

            for (var i = 0; i < pathController.Positions.Length; i++)
            {
                var position  = pathController.Positions[i];
                var direction = pathController.Directions[i];
                GizmosUtils.ThicknessArrow(position, direction, Color.yellow, arrowScale);
            }

            // 決定是否畫出封閉的路徑
            var waypointsLength = path.isClosed ? path.Waypoints.Length : path.Waypoints.Length - 1;

            for (var i = 0; i < waypointsLength; i++)
            {
                var nextIndex = (i + 1) % path.Waypoints.Length;
                Gizmos.DrawLine(path.Waypoints[i], path.Waypoints[nextIndex]);
                GizmosUtils.Arrow(path.Waypoints[i], path.Waypoints[nextIndex] - path.Waypoints[i], Color.cyan);
            }
        }


        public void UpdateGizmo()
        {
            if (pathController == null) return;
            pathController.UpdatePath();
        }
    }
}