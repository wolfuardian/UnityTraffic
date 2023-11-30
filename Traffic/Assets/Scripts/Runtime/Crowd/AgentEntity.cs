using System;
using UnityEngine;
using UnityEngine.AI;

namespace Runtime.Crowd
{
    public class AgentEntity : MonoBehaviour
    {
        
        public NavMeshAgent navMeshAgent;
        public bool shouldDestroy;
        private float _turningRadius;

        public Action<AgentEntity> onAgentExited;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            DestroyWhenReachedGoal();
        }

        private void DestroyWhenReachedGoal()
        {
            if (!shouldDestroy || navMeshAgent.pathPending ||
                !(navMeshAgent.remainingDistance < _turningRadius)) return;

            Destroy(gameObject);
        }

        public void SetSpeed(float speed) => navMeshAgent.speed = speed;
        public void SetTurningRadius(float turningRadius) => _turningRadius = turningRadius;
        public void SetStoppingDistance(float stoppingDistance) => navMeshAgent.stoppingDistance = stoppingDistance;
        public void SetDestination(Vector3 destination) => navMeshAgent.SetDestination(destination);
        private void OnDestroy() => onAgentExited?.Invoke(this);
    }
}