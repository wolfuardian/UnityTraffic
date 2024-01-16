using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class AgentTracker : MonoBehaviour
    {
        [SerializeField]                     private CrowdPath        crowdPath;
        [SerializeField]                     private GameObject       previousTarget;
        [SerializeField]                     private GameObject       target;
        [SerializeField]                     private Vector3          desiredPosition;
        [SerializeField]                     private int              waypointIndex;
        [SerializeField] [Range(0.0f, 1.0f)] private float            globalJourney;
        [SerializeField] [Range(0.0f, 1.0f)] private float            localJourney;
        [SerializeField]                     private float            localDistance;
        [SerializeField]                     private float            remainingDistance;
        [SerializeField]                     private bool             isTrackable;
        private                                      List<GameObject> _waypoints = new List<GameObject>();
        private                                      AgentEntity      _agentEntity;
        private                                      float            _turningRadius;


        public void SetWaypoints(List<GameObject> waypoints)     => _waypoints = waypoints;
        public void SetAgentEntity(AgentEntity    agentEntity)   => _agentEntity = agentEntity;
        public void SetTurningRadius(float        turningRadius) => _turningRadius = turningRadius;


        private void Start()
        {
            InitializeWaypoint();
        }

        private void Update()
        {
            if (!isTrackable) return;
            MoveToNextWaypoint();
            CalculateJourney();
        }


        public void SetCrowdPath(CrowdPath path)
        {
            // Set variable
            crowdPath = path;

            // Handle
            SetWaypoints(crowdPath.waypoints);
            waypointIndex = 0;
            MoveToNextWaypointImmediately();
        }

        public void InitializeWaypoint()
        {
            _waypoints = crowdPath.waypoints;
            if (_waypoints != null && _waypoints.Count > 0)
            {
                waypointIndex   = 0;
                target          = _waypoints[0];
                previousTarget  = target;
                desiredPosition = GetRandomPointInRadius(target);
                localDistance   = Vector3.Distance(previousTarget.transform.position, desiredPosition);
                SetAgentDestination();
                isTrackable = true;
            }
            else
            {
                Debug.LogError("Waypoints list is empty or null! Make sure you have set the waypoints.");
            }
        }

        private void MoveToNextWaypoint()
        {
            if (Vector3.Distance(transform.position, desiredPosition) < _turningRadius) FindNextWaypoint();
            SetAgentDestination();
        }

        private void MoveToNextWaypointImmediately()
        {
            FindNextWaypoint();
            SetCurrentWaypointAsTarget();
            SetAgentDestination();
        }

        private void FindNextWaypoint()
        {
            if (target == null)
            {
                target = _waypoints[0];
            }

            previousTarget = target;
            if (previousTarget == target) desiredPosition = GetRandomPointInRadius(target);

            if (waypointIndex < _waypoints.Count - 1)
            {
                waypointIndex++;
            }

            if (waypointIndex < _waypoints.Count && waypointIndex >= 0)
            {
                target = _waypoints[waypointIndex];
            }
        }

        public void SetCurrentWaypointAsTarget()
        {
            target          = _waypoints[waypointIndex];
            desiredPosition = GetRandomPointInRadius(target);
        }

        public void SetAgentDestination() => _agentEntity.SetDestination(desiredPosition);

        private void CalculateJourney()
        {
            waypointIndex     = _waypoints.IndexOf(target);
            remainingDistance = _agentEntity.NavMeshAgent.remainingDistance;
            localDistance     = Vector3.Distance(previousTarget.transform.position, desiredPosition);
            localJourney      = Mathf.Clamp(1 - remainingDistance / localDistance, 0f, 1f);
            if (previousTarget == target) localJourney = 1f;

            globalJourney = (waypointIndex + localJourney) / _waypoints.Count;

            if (Math.Abs(globalJourney - 1f) < 0.001f)
            {
                _agentEntity.SetShouldDestroy(true);
            }
        }

        private static Vector3 GetRandomPointInRadius(GameObject point)
        {
            var crowdPathPoint = point.GetComponent<CrowdPathPoint>();
            if (crowdPathPoint == null)
            {
                Debug.LogWarning("CrowdPathPoint component is missing on the GameObject: " + point.name);
                return point.transform.position;
            }

            var radius          = crowdPathPoint.allowableRadius;
            var randomDirection = Random.insideUnitSphere * radius;
            var position        = point.transform.position;
            randomDirection   += position;
            randomDirection.y =  position.y;
            return randomDirection;
        }

        private void OnDrawGizmos()
        {
            if (target == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, desiredPosition);
        }
    }
}