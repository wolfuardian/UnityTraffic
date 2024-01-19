using CrowdSample.Scripts.Utils;
using UnityEngine;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    [RequireComponent(typeof(Waypoint))]
    [ExecuteInEditMode]
    public class WaypointGizmos : MonoBehaviour
    {
        [SerializeField] private Waypoint waypoint;

        public  Waypoint Waypoint => waypoint ??= GetComponent<Waypoint>();
        private Vector3  prevPosition;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Waypoint == null) return;

            var position = Waypoint.transform.position;
            if (prevPosition != position)
            {
                UnityEditorUtils.UpdateAllGizmos();
                prevPosition = position;
            }

            DrawWaypointGizmo(position);
        }

        private void DrawWaypointGizmo(Vector3 position)
        {
            GizmosUtils.Polygon(position, Color.green, Waypoint.Radius, 32);
        }
#endif
    }
}