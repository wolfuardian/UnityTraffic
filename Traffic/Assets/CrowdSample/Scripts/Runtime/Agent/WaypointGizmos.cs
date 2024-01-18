using UnityEngine;
using CrowdSample.Scripts.Utils;

namespace CrowdSample.Scripts.Runtime.Agent
{
    [RequireComponent(typeof(Waypoint))]
    [ExecuteInEditMode]
    public class WaypointGizmos : MonoBehaviour
    {
        public Waypoint waypoint;

#if UNITY_EDITOR
        private void Awake()
        {
            if (waypoint == null) waypoint = GetComponent<Waypoint>();
        }

        private void OnDrawGizmos()
        {
            if (waypoint == null) return;
            var position = waypoint.transform.position;

            GizmosUtils.Polygon(position, Color.green, waypoint.Radius, 32);
        }
#endif
    }
}