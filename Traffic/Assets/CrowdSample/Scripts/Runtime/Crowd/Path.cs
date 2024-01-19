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

        public float   GetTotalLength()        => PathCalculator.GetTotalLength(Waypoints, ClosedPath);
        public Vector3 GetPositionAt(float  t) => PathCalculator.GetPositionAt(Waypoints, ClosedPath, t);
        public Vector3 GetDirectionAt(float t) => PathCalculator.GetDirectionAt(Waypoints, ClosedPath, t);
    }
}