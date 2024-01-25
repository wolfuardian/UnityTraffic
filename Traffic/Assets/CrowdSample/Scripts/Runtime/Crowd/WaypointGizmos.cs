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

        public Vector3 PrevPosition { get; set; }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Waypoint == null) return;

            var position = Waypoint.transform.position;
            if (PrevPosition != position)
            {
                UnityEditorUtils.UpdateAll();
                PrevPosition = position;
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