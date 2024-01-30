using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using CrowdSample.Scripts.Utils;
using UnityEditor;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace CrowdSample.Scripts.Crowd
{
    public class PathNavigator : MonoBehaviour
    {
        #region Field Declarations

        public enum NavigationMode
        {
            Loop,
            PingPong,
            Once
        }

        [SerializeField] private NavigationMode m_navigationMode;
        [SerializeField] private NavMeshAgent   m_navMeshAgent;
        [SerializeField] private List<Waypoint> m_waypoints;
        [SerializeField] private int            m_targetID;
        [SerializeField] private bool           m_movingForward = true;
        [SerializeField] private bool           m_shouldDestroy = true;

        [SerializeField] private float m_reachedThreshold = 0.1f;

        [SerializeField] private int m_queueIndex;
        [SerializeField] private int m_queueTotalCount;


        public UnityAction                DestroyEvent;
        public UnityAction<PathNavigator> QueueStateEvent;

        #endregion

        #region Properties

        public NavigationMode navigationMode
        {
            get => m_navigationMode;
            set => m_navigationMode = value;
        }

        public NavMeshAgent navMeshAgent
        {
            get => m_navMeshAgent;
            set => m_navMeshAgent = value;
        }

        public List<Waypoint> waypoints
        {
            get => m_waypoints;
            set => m_waypoints = value;
        }

        public int targetID
        {
            get => m_targetID;
            set => m_targetID = value;
        }

        public bool movingForward
        {
            get => m_movingForward;
            set => m_movingForward = value;
        }

        public bool shouldDestroy
        {
            get => m_shouldDestroy;
            set => m_shouldDestroy = value;
        }

        public float reachedThreshold
        {
            get => m_reachedThreshold;
            set => m_reachedThreshold = value;
        }

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

        #endregion

        #region Unity Methods

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>() ?? gameObject.AddComponent<NavMeshAgent>();
        }

        private void Start()
        {
            if (waypoints.Count > 0)
            {
                var radius      = waypoints[targetID].radius;
                var destination = waypoints[targetID].transform.position;
                reachedThreshold         = radius;
                navMeshAgent.destination = ScatterDestination(destination, radius);
            }
        }

        private void Update()
        {
            if (ShouldReturnEarly())
                return;

            TriggerQueueStateEvent();

            CalculateAndSetAvoidancePriority();


            if (IsDestinationReached())
            {
                HandleDestinationReached();
            }
            else
            {
                ContinueToNextWaypoint();
            }
        }

        private void LateUpdate()
        {
            HandleGoalReached();
        }

        private void OnDestroy()
        {
            DestroyEvent?.Invoke();
        }

        #endregion

        #region Private Methods

        private bool ShouldReturnEarly()
        {
            return waypoints.Count == 0 || navMeshAgent.pathPending ||
                   navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance;
        }

        private void TriggerQueueStateEvent()
        {
            // 觸發QueueStateEvent以通知監聽者隊列狀態的變化
            QueueStateEvent?.Invoke(this);
        }

        private void CalculateAndSetAvoidancePriority()
        {
            // 根據隊列索引和總數計算避免優先級（0-99）
            var priorityRatio     = queueIndex / (float)queueTotalCount;
            var avoidancePriority = (int)(priorityRatio * 100);
            navMeshAgent.avoidancePriority = avoidancePriority;
        }

        private static Vector3 ScatterDestination(Vector3 dest, float r)
        {
            return dest + new Vector3(Random.insideUnitCircle.x * r, 0f, Random.insideUnitCircle.y * r);
        }

        private void HandleDestinationReached()
        {
            MoveToNextWaypoint();
        }

        private void HandleGoalReached()
        {
            if (IsGoalReached() && shouldDestroy)
            {
                Destroy(gameObject);
            }
        }

        private void MoveToNextWaypoint()
        {
            switch (navigationMode)
            {
                case NavigationMode.Loop:
                    if (movingForward)
                    {
                        targetID = (targetID + 1) % waypoints.Count;
                    }
                    else
                    {
                        targetID = (targetID - 1) % waypoints.Count;
                        if (targetID < 0)
                        {
                            targetID = waypoints.Count - 1;
                        }
                    }

                    break;

                case NavigationMode.PingPong:
                    if (targetID <= 0)
                    {
                        movingForward = true;
                    }
                    else if (targetID >= waypoints.Count - 1)
                    {
                        movingForward = false;
                    }

                    targetID += movingForward ? 1 : -1;
                    break;

                case NavigationMode.Once:
                    if (movingForward)
                    {
                        if (targetID < waypoints.Count - 1)
                        {
                            targetID++;
                        }
                    }
                    else
                    {
                        if (targetID > 0)
                        {
                            targetID--;
                        }
                    }

                    break;
            }

            var radius      = waypoints[targetID].radius;
            var destination = waypoints[targetID].transform.position;
            reachedThreshold         = radius;
            navMeshAgent.destination = ScatterDestination(destination, radius);
        }

        private bool IsDestinationReached()
        {
            return navMeshAgent.remainingDistance < reachedThreshold;
        }

        private bool IsGoalReached()
        {
            var remainingDistance = transform.position - waypoints[targetID].transform.position;
            return remainingDistance.magnitude < reachedThreshold && targetID == waypoints.Count - 1;
        }

        private void ContinueToNextWaypoint()
        {
            var targetPosition = waypoints[targetID].transform.position;
            navMeshAgent.destination = targetPosition;
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

            Handles.Label(position, "TargetID: " + targetID, style);
        }

        #endregion
    }
}