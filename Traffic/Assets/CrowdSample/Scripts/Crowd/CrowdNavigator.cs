using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using UnityEngine.Events;
using CrowdSample.Scripts.Utils;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace CrowdSample.Scripts.Crowd
{
    public class CrowdNavigator : MonoBehaviour
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
        [SerializeField] private List<CrowdWaypoint> m_waypoints;
        [SerializeField] private int            m_spawnID;
        [SerializeField] private int            m_targetPointID;
        [SerializeField] private bool           m_movingForward = true;
        [SerializeField] private bool           m_shouldDestroy = true;

        [SerializeField] private float m_reachedThreshold = 0.1f;

        [SerializeField] private int m_queueIndex;
        [SerializeField] private int m_queueTotalCount;


        public UnityAction                 DestroyEvent;
        public UnityAction<CrowdNavigator> QueueStateEvent;

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

        public List<CrowdWaypoint> waypoints
        {
            get => m_waypoints;
            set => m_waypoints = value;
        }

        public int spawnID
        {
            get => m_spawnID;
            set => m_spawnID = value;
        }

        public int targetPointID
        {
            get => m_targetPointID;
            set => m_targetPointID = value;
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
                var radius0     = waypoints[0].radius;
                var radius1     = waypoints[targetPointID].radius;
                var destination = waypoints[targetPointID].transform.position;
                reachedThreshold         = radius1;
                navMeshAgent.destination = ScatterDestination(destination,        radius1);
                transform.position       = ScatterDestination(transform.position, radius0);
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
                navMeshAgent.destination = transform.position + transform.forward * 0.1f;
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
                   navMeshAgent.remainingDistance > reachedThreshold;
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
                        targetPointID = (targetPointID + 1) % waypoints.Count;
                    }
                    else
                    {
                        targetPointID = (targetPointID - 1) % waypoints.Count;
                        if (targetPointID < 0)
                        {
                            targetPointID = waypoints.Count - 1;
                        }
                    }

                    break;

                case NavigationMode.PingPong:
                    if (targetPointID <= 0)
                    {
                        movingForward = true;
                    }
                    else if (targetPointID >= waypoints.Count - 1)
                    {
                        movingForward = false;
                    }

                    targetPointID += movingForward ? 1 : -1;
                    break;

                case NavigationMode.Once:
                    if (movingForward)
                    {
                        if (targetPointID < waypoints.Count - 1)
                        {
                            targetPointID++;
                        }
                    }
                    else
                    {
                        if (targetPointID > 0)
                        {
                            targetPointID--;
                        }
                    }

                    break;
            }

            var radius      = waypoints[targetPointID].radius;
            var destination = waypoints[targetPointID].transform.position;
            reachedThreshold         = radius;
            navMeshAgent.destination = ScatterDestination(destination, radius);
        }

        private bool IsDestinationReached()
        {
            return navMeshAgent.remainingDistance < reachedThreshold;
        }

        private bool IsGoalReached()
        {
            var remainingDistance = transform.position - waypoints[targetPointID].transform.position;
            return remainingDistance.magnitude < reachedThreshold && targetPointID == waypoints.Count - 1;
        }

        private void ContinueToNextWaypoint()
        {
            var targetPosition = waypoints[targetPointID].transform.position;
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

            var position = transform.position + Vector3.up * 2f;

            var direction = navMeshAgent.destination - position;
            GizmosUtils.Arrow(position, direction, Color.red);

            var style = new GUIStyle
            {
                normal    = { textColor = Color.white },
                alignment = TextAnchor.UpperLeft,
                fontSize  = 9
            };

            Handles.Label(position, "ID: " + spawnID, style);
        }

        #endregion
    }
}