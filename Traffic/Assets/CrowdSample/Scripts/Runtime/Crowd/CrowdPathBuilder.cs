using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using CrowdSample.Scripts.Utils;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    [RequireComponent(typeof(CrowdPathController))]
    [ExecuteInEditMode]
    public class CrowdPathBuilder : MonoBehaviour, IUpdateReceiver
    {
        #region Field Declarations

        public EditMode editMode = EditMode.None;

        [SerializeField] private CrowdPathController crowdPathController;
        [SerializeField] private List<Transform>     waypoints;
        [SerializeField] private Vector2             arrowScale = new Vector2(2f, 2f);

        public enum EditMode
        {
            None = 0,
            Add  = 1
        }

        #endregion

        #region Properties

        public CrowdPathController CrowdPathController => crowdPathController ??= GetComponent<CrowdPathController>();
        public IEnumerable<Transform> Waypoints => waypoints ??= new List<Transform>();
        public Vector2 ArrowScale => arrowScale;
        public bool IsPathValid => crowdPathController.PointsSet.Count >= 2;

        #endregion


        #region Unity Methods

#if UNITY_EDITOR
        private void Awake()
        {
            if (crowdPathController == null) crowdPathController = GetComponent<CrowdPathController>();
        }

        private void OnDrawGizmos()
        {
            if (!IsPathValid) return;

            DrawAgentSpawnPoints();
            DrawPathWaypoints();
        }
#endif

        #endregion

        #region Public Methods

        public void FetchWaypoints()
        {
            waypoints = transform.GetComponentsInChildren<Waypoint>().Select(wp => wp.transform).ToList();
        }

        #endregion

        #region Implementation Methods

        public void UpdateImmediately()
        {
            FetchWaypoints();
        }

        #endregion

        #region Debug and Visualization Methods

        private void DrawAgentSpawnPoints()
        {
            foreach (var spawnData in crowdPathController.AgentSpawnData)
            {
                GizmosUtils.ThicknessArrow(spawnData.position, spawnData.direction, Color.yellow, arrowScale);
            }
        }

        private void DrawPathWaypoints()
        {
            var waypointCount        = waypoints.Count;
            var actualWaypointsCount = crowdPathController.IsClosedPath ? waypointCount : waypointCount - 1;

            for (var i = 0; i < actualWaypointsCount; i++)
            {
                var nextIndex = (i + 1) % waypointCount;
                if (waypoints[i] == null || waypoints[nextIndex] == null) continue;

                Gizmos.DrawLine(waypoints[i].position, waypoints[nextIndex].position);
                GizmosUtils.Arrow(waypoints[i].position, waypoints[nextIndex].position - waypoints[i].position,
                    Color.cyan);
            }
        }

        #endregion
    }
}