using UnityEngine;
using System.Collections.Generic;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class Path : MonoBehaviour
    {
        [SerializeField] private bool            closedPath;
        [SerializeField] private List<Transform> waypoints;

        public bool            ClosedPath => closedPath;
        public List<Transform> Waypoints  => waypoints;

        public void SetClosedPath(bool           value) => closedPath = value;
        public void SetWaypoints(List<Transform> value) => waypoints = value;

        public float   GetTotalLength()        => PathResolver.GetTotalLength(Waypoints, ClosedPath);
        public Vector3 GetPositionAt(float  t) => PathResolver.GetPositionAt(Waypoints, ClosedPath, t);
        public Vector3 GetDirectionAt(float t) => PathResolver.GetDirectionAt(Waypoints, ClosedPath, t);

#if UNITY_EDITOR
        private void OnValidate()
        {
            FetchWaypoints();
        }

        public void FetchWaypoints()
        {
            SetWaypoints(new List<Transform>());

            var children = transform.GetComponentsInChildren<Waypoint>();
            if (children.Length <= 0) return;

            foreach (var child in children)
            {
                Waypoints.Add(child.transform);
            }
        }
#endif
    }
}