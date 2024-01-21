using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class AgentTracker : MonoBehaviour
    {
        [SerializeField]                     private Path            path;
        [SerializeField]                     private Transform       previousTarget;
        [SerializeField]                     private Transform       target;
        [SerializeField]                     private Vector3         desiredPosition;
        [SerializeField]                     private int             waypointIndex;
        [SerializeField] [Range(0.0f, 1.0f)] private float           globalJourney;
        [SerializeField] [Range(0.0f, 1.0f)] private float           localJourney;
        [SerializeField]                     private float           localDistance;
        [SerializeField]                     private float           remainingDistance;
        [SerializeField]                     private bool            isTrackable;
        private                                      List<Transform> waypoints = new List<Transform>();
        private                                      AgentEntity     agentEntity;
        private                                      float           turningRadius;


        public void SetWaypoints(List<Transform> newWaypoints)     => waypoints = newWaypoints;
        public void SetAgentEntity(AgentEntity   newAgentEntity)   => agentEntity = newAgentEntity;
        public void SetTurningRadius(float       newTurningRadius) => turningRadius = newTurningRadius;


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


        public void SetPath(Path newPath)
        {
            // Set variable
            path = newPath;

            // Handle
            SetWaypoints(newPath.Waypoints);
            waypointIndex = 0;
            MoveToNextWaypointImmediately();
        }

        public void InitializeWaypoint()
        {
            waypoints = path.Waypoints;
            if (waypoints != null && waypoints.Count > 0)
            {
                waypointIndex   = 0;
                target          = waypoints[0];
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
            if (Vector3.Distance(transform.position, desiredPosition) < turningRadius) FindNextWaypoint();
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
                target = waypoints[0];
            }

            previousTarget = target;
            if (previousTarget == target) desiredPosition = GetRandomPointInRadius(target);

            if (waypointIndex < waypoints.Count - 1)
            {
                waypointIndex++;
            }

            if (waypointIndex < waypoints.Count && waypointIndex >= 0)
            {
                target = waypoints[waypointIndex];
            }
        }

        public void SetCurrentWaypointAsTarget()
        {
            target          = waypoints[waypointIndex];
            desiredPosition = GetRandomPointInRadius(target);
        }

        public void SetAgentDestination() => agentEntity.SetDestination(desiredPosition);

        private void CalculateJourney()
        {
            waypointIndex     = waypoints.IndexOf(target);
            remainingDistance = agentEntity.NavMeshAgent.remainingDistance;
            localDistance     = Vector3.Distance(previousTarget.transform.position, desiredPosition);
            localJourney      = Mathf.Clamp(1 - remainingDistance / localDistance, 0f, 1f);
            if (previousTarget == target) localJourney = 1f;

            globalJourney = (waypointIndex + localJourney) / waypoints.Count;

            if (Math.Abs(globalJourney - 1f) < 0.001f)
            {
                agentEntity.SetShouldDestroy(true);
            }
        }

        private static Vector3 GetRandomPointInRadius(Component point)
        {
            var crowdPathPoint = point.GetComponent<Waypoint>();
            if (crowdPathPoint == null)
            {
                Debug.LogWarning("CrowdPathPoint component is missing on the GameObject: " + point.name);
                return point.transform.position;
            }

            var radius          = crowdPathPoint.Radius;
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