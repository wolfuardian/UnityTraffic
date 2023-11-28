using UnityEngine;
using UnityEngine.AI;

namespace Runtime.Crowd
{
    public class AgentEntity : MonoBehaviour
    {
        public NavMeshAgent navMeshAgent;

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public void SetSpeed(float speed)
        {
            navMeshAgent.speed = speed;
        }

        public void SetDestination(Vector3 destination)
        {
            navMeshAgent.SetDestination(destination);
        }
    }
}