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

        public EditMode m_editMode = EditMode.None;

        [SerializeField] private CrowdPathController m_crowdPathController;
        [SerializeField] private List<Transform>     m_waypoints  = new List<Transform>();
        [SerializeField] private Vector2             m_arrowScale = new Vector2(2f, 2f);

        public enum EditMode
        {
            None = 0,
            Add  = 1
        }

        #endregion

        #region Properties

        public CrowdPathController crowdPathController
        {
            get => m_crowdPathController;
            set => m_crowdPathController = value;
        }

        public List<Transform> waypoints
        {
            get => m_waypoints;
            set => m_waypoints = value;
        }

        public Vector2 arrowScale
        {
            get => m_arrowScale;
            set => m_arrowScale = value;
        }

        public bool pathValid => waypoints.Count >= 2;

        #endregion


        #region Unity Methods

#if UNITY_EDITOR
        private void Awake()
        {
            if (m_crowdPathController == null) m_crowdPathController = GetComponent<CrowdPathController>();
        }

        private void OnDrawGizmos()
        {
            if (!pathValid) return;

            DrawAgentSpawnPoints();
            DrawPathWaypoints();
        }
#endif

        #endregion

        #region Public Methods

        public void FetchWaypoints()
        {
            m_waypoints = transform.GetComponentsInChildren<Waypoint>().Select(wp => wp.transform).ToList();
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
            foreach (var spawnData in m_crowdPathController.spawnPointsData)
            {
                GizmosUtils.ThicknessArrow(spawnData.m_position, spawnData.m_direction, Color.yellow, m_arrowScale);
            }
        }

        private void DrawPathWaypoints()
        {
            var waypointCount        = m_waypoints.Count;
            var actualWaypointsCount = m_crowdPathController.closedLoop ? waypointCount : waypointCount - 1;

            for (var i = 0; i < actualWaypointsCount; i++)
            {
                var nextIndex = (i + 1) % waypointCount;
                if (m_waypoints[i] == null || m_waypoints[nextIndex] == null) continue;

                Gizmos.DrawLine(m_waypoints[i].position, m_waypoints[nextIndex].position);
                GizmosUtils.Arrow(m_waypoints[i].position, m_waypoints[nextIndex].position - m_waypoints[i].position,
                    Color.cyan);
            }
        }

        #endregion
    }
}