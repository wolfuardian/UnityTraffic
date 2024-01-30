using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Collections.Generic;
using CrowdSample.Scripts.Utils;
using Random = UnityEngine.Random;

namespace CrowdSample.Scripts.Crowd
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class CrowdNavigate : MonoBehaviour
    {
        #region Field Declarations

        public enum NavigateMode
        {
            Loop,
            PingPong,
            Once,
            Custom
        }

        [SerializeField] private NavigateMode m_navigateMode = NavigateMode.Loop;

        // References
        [SerializeField] private CrowdPath        m_path;
        [SerializeField] private NavMeshAgent     m_navMeshAgent;
        [SerializeField] private List<Waypoint>   m_waypoints   = new List<Waypoint>();
        [SerializeField] private List<SpawnPoint> m_spawnPoints = new List<SpawnPoint>();

        // PathNavigate
        [SerializeField] private int  m_currentIndex;
        [SerializeField] private int  m_targetIndex;
        [SerializeField] private int  m_forceTargetIndex;
        [SerializeField] private bool m_stopOnNextPoint;
        [SerializeField] private bool m_shouldDestroyOnGoal;
        [SerializeField] private bool m_reverse;
        [SerializeField] private bool m_closedLoop;

        //
        [SerializeField] private int m_queueTotalCount;
        [SerializeField] private int m_queueIndex;

        // NavMeshAgent
        [SerializeField] private float   m_scatterRadius;
        [SerializeField] private Vector3 m_destination;

        [SerializeField] private float m_localDistance;
        [SerializeField] private float m_localJourney;
        [SerializeField] private float m_globalJourney;
        [SerializeField] private float m_remainingDistance;

        [SerializeField] private float m_percentage;
        [SerializeField] private int   m_priority;

        [SerializeField] private bool m_spawnable = true;

        private bool initialized;
        private bool firstFrame = true;

        #endregion

        #region Properties

        // References
        public CrowdPath path
        {
            get => m_path;
            set => m_path = value;
        }

        public NavMeshAgent navMeshAgent
        {
            get => m_navMeshAgent;
            set => m_navMeshAgent = value;
        }

        public NavigateMode navigateMode
        {
            get => m_navigateMode;
            set => m_navigateMode = value;
        }

        public List<Waypoint> waypoints
        {
            get => m_waypoints;
            set => m_waypoints = value;
        }

        public List<SpawnPoint> spawnPoints
        {
            get => m_spawnPoints;
            set => m_spawnPoints = value;
        }

        // PathNavigate
        public int currentIndex
        {
            get => m_currentIndex;
            set => m_currentIndex = value;
        }

        public int targetIndex
        {
            get => m_targetIndex;
            set => m_targetIndex = value;
        }

        public int forceTargetIndex
        {
            get => m_forceTargetIndex;
            set => m_forceTargetIndex = value;
        }

        public bool stopOnNextPoint
        {
            get => m_stopOnNextPoint;
            set => m_stopOnNextPoint = value;
        }

        public bool shouldDestroyOnGoal
        {
            get => m_shouldDestroyOnGoal;
            set => m_shouldDestroyOnGoal = value;
        }

        public bool reverse
        {
            get => m_reverse;
            set => m_reverse = value;
        }

        public bool closedLoop
        {
            get => m_closedLoop;
            set => m_closedLoop = value;
        }

        //

        public int queueIndex
        {
            get => m_queueIndex;
            set => m_queueIndex = value;
        }

        public int queueTotalCount
        {
            get => m_queueTotalCount;
            set => m_queueTotalCount = value;
        }

        public float percentage
        {
            get => m_percentage;
            set => m_percentage = value;
        }

        public int priority
        {
            get => m_priority;
            set => m_priority = value;
        }

        // NavMeshAgent
        public float scatterRadius
        {
            get => m_scatterRadius;
            set => m_scatterRadius = value;
        }

        public Vector3 destination
        {
            get => m_destination;
            set => m_destination = value;
        }

        public float localDistance
        {
            get => m_localDistance;
            set => m_localDistance = value;
        }

        public float localJourney
        {
            get => m_localJourney;
            set => m_localJourney = value;
        }

        public float globalJourney
        {
            get => m_globalJourney;
            set => m_globalJourney = value;
        }

        public float remainingDistance
        {
            get => m_remainingDistance;
            set => m_remainingDistance = value;
        }

        public UnityAction DestroyEvent;

        public bool spawnable
        {
            get => m_spawnable;
            set => m_spawnable = value;
        }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>() ?? gameObject.AddComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (stopOnNextPoint)
            {
                return;
            }

            if (waypoints.Count == 0)
            {
                return;
            }

            if (navMeshAgent.pathPending || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                return;
            }

            if (!firstFrame)
            {
                UpdateCurrentIndex(reverse);

                UpdateTargetIndex();
            }
            // if (!firstFrame)
            // {
            //     Debug.Log("UpdateCurrentIndex");
            //     UpdateIndexBasedOnDirection(reverse);
            // }


            destination = waypoints[targetIndex].transform.position;
            destination = ScatterDestination(destination, scatterRadius);

            navMeshAgent.destination = destination;

            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
            {
                navMeshAgent.destination = path.waypoints[targetIndex].transform.position;

                targetIndex = (targetIndex + 1) % path.waypoints.Count;
            }

            initialized = true;
            firstFrame  = false;
        }

        private void LateUpdate()
        {
            if (!initialized)
            {
                return;
            }

            if (shouldDestroyOnGoal && currentIndex == targetIndex)
            {
                // Destroy(gameObject);
            }

            var self   = transform;
            var parent = self.parent;
            queueIndex                     = parent.GetComponent<CrowdSpawner>().trackingAgents.IndexOf(self);
            queueTotalCount                = parent.GetComponent<CrowdSpawner>().trackingAgents.Count;
            percentage                     = queueIndex / (float)queueTotalCount;
            priority                       = (int)(percentage * 100);
            navMeshAgent.avoidancePriority = priority;
        }

        private void OnDestroy()
        {
            DestroyEvent?.Invoke();
        }

        #endregion

        #region Private Methods

        private void UpdateCurrentIndex(bool state)
        {
            if (state)

                if (currentIndex <= targetIndex)
                {
                    currentIndex = targetIndex;
                }
                else
                {
                    currentIndex--;
                }
            else
            {
                if (currentIndex >= targetIndex)
                {
                    currentIndex = targetIndex;
                }
                else
                {
                    currentIndex++;
                }
            }
        }

        private void UpdateIndexBasedOnDirection(bool state)
        {
            targetIndex += state ? -1 : 1;
        }

        private void UpdateTargetIndex()
        {
            switch (navigateMode)
            {
                case NavigateMode.Loop:
                    Loop();
                    break;
                case NavigateMode.PingPong:
                    PingPong();
                    break;
                case NavigateMode.Once:
                    Once();
                    break;
                case NavigateMode.Custom:
                    Custom();
                    break;
            }
        }

        private static Vector3 ScatterDestination(Vector3 dest, float r)
        {
            return dest + new Vector3(Random.insideUnitCircle.x * r, 0f, Random.insideUnitCircle.y * r);
        }

        private void Loop()
        {
            if (targetIndex >= waypoints.Count)
            {
                targetIndex = 0;
            }
            else if (targetIndex < 0)
            {
                targetIndex = waypoints.Count - 1;
            }
        }

        private void PingPong()
        {
            if (targetIndex >= waypoints.Count - 1)
            {
                reverse = true;
            }
            else if (targetIndex <= 0)
            {
                reverse = false;
            }
        }

        private void Once()
        {
            targetIndex = Mathf.Clamp(targetIndex, 0, waypoints.Count - 1);
        }

        private void Custom()
        {
            if (targetIndex == forceTargetIndex)
            {
                return;
            }

            var direction = targetIndex < forceTargetIndex ? 1 : -1;
            targetIndex += direction;

            if ((direction == 1 && targetIndex > forceTargetIndex) ||
                (direction == -1 && targetIndex < forceTargetIndex))
            {
                targetIndex = forceTargetIndex;
            }
        }

        #endregion

        #region Debug and Visualization Methods

        private void OnDrawGizmos()
        {
            if (navMeshAgent == null)
            {
                return;
            }

            var position = transform.position;

            var direction = navMeshAgent.destination - position;
            GizmosUtils.Arrow(position, direction, Color.red);

            var style = new GUIStyle
            {
                normal    = { textColor = Color.white },
                alignment = TextAnchor.UpperLeft
            };

            Handles.Label(position, "TargetID: " + targetIndex, style);
        }

        #endregion
    }
}