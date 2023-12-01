using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class AgentTracker : MonoBehaviour
    {
        [SerializeField] private GameObject previousTarget;
        [SerializeField] private GameObject target;
        [SerializeField] private Vector3 desiredPosition;
        [SerializeField] private int waypointIndex;
        [SerializeField] [Range(0.0f, 1.0f)] private float globalJourney;
        [SerializeField] [Range(0.0f, 1.0f)] private float localJourney;
        [SerializeField] private float localDistance;
        [SerializeField] private float remainingDistance;
        [SerializeField] private bool isTrackable;
        private List<GameObject> _waypoints = new List<GameObject>();
        private AgentEntity _agentEntity;
        private float _turningRadius;


        public void SetWaypoints(List<GameObject> waypoints) => _waypoints = waypoints;
        public void SetAgentEntity(AgentEntity agentEntity) => _agentEntity = agentEntity;
        public void SetTurningRadius(float turningRadius) => _turningRadius = turningRadius;


        private void Start()
        {
            if (_waypoints != null && _waypoints.Count > 0)
            {
                waypointIndex = 0;
                target = _waypoints[0];
                previousTarget = target;
                desiredPosition = GetRandomPointInRadius(target);
                localDistance = Vector3.Distance(previousTarget.transform.position, desiredPosition);
                SetAgentDestination();
                isTrackable = true;
            }
            else
            {
                Debug.LogError("Waypoints list is empty or null!");
            }
        }

        private void Update()
        {
            if (!isTrackable) return;
            MoveToward();
            CalculateJourney();
        }

        private void MoveToward()
        {
            if (Vector3.Distance(transform.position, desiredPosition) < _turningRadius)
            {
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

            SetAgentDestination();
        }

        private void SetAgentDestination()
        {
            _agentEntity.SetDestination(desiredPosition);
        }

        private void CalculateJourney()
        {
            waypointIndex = _waypoints.IndexOf(target);
            remainingDistance = _agentEntity.navMeshAgent.remainingDistance;
            localDistance = Vector3.Distance(previousTarget.transform.position, desiredPosition);
            localJourney = Mathf.Clamp(1 - remainingDistance / localDistance, 0f, 1f);
            if (previousTarget == target) localJourney = 1f;

            globalJourney = (waypointIndex + localJourney) / _waypoints.Count;

            if (Math.Abs(globalJourney - 1f) < 0.001f)
            {
                _agentEntity.shouldDestroy = true;
            }
        }

        private static Vector3 GetRandomPointInRadius(GameObject point)
        {
            var crowdPathPoint = point.GetComponent<CrowdPathPoint>();
            if (crowdPathPoint != null)
            {
                var radius = crowdPathPoint.allowableRadius;
                var randomDirection = Random.insideUnitSphere * radius;
                var position = point.transform.position;
                randomDirection += position;
                randomDirection.y = position.y;
                return randomDirection;
            }

            return point.transform.position;
        }

        void OnDrawGizmos()
        {
            if (target != null)
            {
                Gizmos.color = Color.red;

                Gizmos.DrawLine(transform.position, desiredPosition);
            }
        }
    }
}