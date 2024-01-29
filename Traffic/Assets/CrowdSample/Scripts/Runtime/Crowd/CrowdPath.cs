using System.Linq;
using UnityEngine;
using CrowdSample.Scripts.Utils;
using System.Collections.Generic;
using CrowdSample.Scripts.Runtime.Data;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    [ExecuteInEditMode]
    public class CrowdPath : MonoBehaviour, IUpdateReceiver
    {
        #region Field Declarations

        [SerializeField] private List<Waypoint> m_waypoints  = new List<Waypoint>();
        [SerializeField] private float          m_arrowScale = 1f;

        public enum EditMode
        {
            None = 0,
            Add  = 1
        }

        #endregion

        #region Properties

        public List<Waypoint> waypoints
        {
            get => m_waypoints;
            set => m_waypoints = value;
        }

        public EditMode editMode { get; set; }

        public CrowdConfig crowdConfig { get; set; }

        public List<SpawnPoint> spawnPoints { get; set; }

        #endregion

        #region Implementation Methods

        public void UpdateImmediately()
        {
            waypoints = GetWaypoints();

            if (crowdConfig == null)
            {
                return;
            }

            if (waypoints.Count < 2)
            {
                return;
            }

            spawnPoints = GetSpawnPoints();
        }

        #endregion

        #region Unity Methods

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateImmediately();
        }

        private void OnDrawGizmos()
        {
            if (waypoints.Count < 2)
            {
                return;
            }

            DrawSpawnArrows();
            DrawPathArrows();
        }
#endif

        #endregion

        #region Public Methods

#if UNITY_EDITOR
        public List<Waypoint> GetWaypoints()
        {
            return transform.GetComponentsInChildren<Waypoint>().ToList();
        }

        public IEnumerable<Transform> GetWaypointsTransform()
        {
            return waypoints.Select(wp => wp.transform).ToList();
        }
#endif

        #endregion

        #region Private Methods

        private List<SpawnPoint> GetSpawnPoints()
        {
            var config      = crowdConfig;
            var points      = waypoints.Select(wp => wp.transform.position).ToList();
            var totalLength = PathResolver.GetTotalLength(points, config.pathClosed);
            var maxCount    = Mathf.FloorToInt(totalLength / config.spacing);

            var spawnCount = config.spawnOnce ? config.instantCount :
                config.useSpacing ? Mathf.Min(config.instantCount, maxCount) : config.instantCount;

            var actualCount = 0;
            for (var i = 0; i < spawnCount; i++)
            {
                var distance = PathResolver.CalculateDistance(config, i, totalLength);
                if (distance > totalLength && !config.pathClosed) break;
                actualCount++;
            }

            var newSpawnPoints = new List<SpawnPoint>();

            for (var i = 0; i < actualCount; i++)
            {
                var distance = PathResolver.CalculateDistance(config, i, totalLength);

                newSpawnPoints.Add(NewSpawnPoint(distance, totalLength));
            }

            return newSpawnPoints;
        }

        private SpawnPoint NewSpawnPoint(float distance, float totalLength)
        {
            var interp        = distance / totalLength;
            var config        = crowdConfig;
            var points        = waypoints.Select(wp => wp.transform.position).ToList();
            var position      = PathResolver.GetPositionAt(config, points, interp);
            var direction     = PathResolver.GetDirectionAt(config, points, interp);
            var pathLocation  = interp * spawnPoints.Count;
            var newSpawnPoint = new SpawnPoint(position, direction, pathLocation);
            return newSpawnPoint;
        }

        #endregion

        #region Debug and Visualization Methods

        private void DrawSpawnArrows()
        {
            foreach (var spawnData in spawnPoints)
            {
                GizmosUtils.ThicknessArrow(spawnData.m_position, spawnData.m_direction, Color.yellow, m_arrowScale);
            }
        }

        private void DrawPathArrows()
        {
            var config = crowdConfig;
            var count  = waypoints.Count;
            var points = waypoints.Select(wp => wp.transform.position).ToList();
            int actualCount;

            if (config.pathClosed)
                actualCount = count;
            else
                actualCount = count - 1;

            for (var index = 0; index < actualCount; index++)
            {
                var nextIndex = (index + 1) % count;
                if (waypoints[index] == null || waypoints[nextIndex] == null)
                {
                    continue;
                }

                var point     = points[index];
                var nextPoint = points[nextIndex];
                Gizmos.DrawLine(point, nextPoint);
                GizmosUtils.Arrow(point, nextPoint - point, Color.cyan);
            }
        }

        #endregion
    }
}