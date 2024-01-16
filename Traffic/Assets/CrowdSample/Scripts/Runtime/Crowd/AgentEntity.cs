using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class AgentEntity : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private bool         shouldDestroy;
        [SerializeField] private string       agentID;
        [SerializeField] private string       licensePlateNumber;
        [SerializeField] private float        originalSpeed;
        [SerializeField] private float        turningRadius;

        // onAgentExited事件在 AgentEntity 被銷毀時觸發。
        public Action<AgentEntity> onAgentExited;

        public NavMeshAgent NavMeshAgent       => navMeshAgent;
        public string       AgentID            => agentID;
        public string       LicensePlateNumber => licensePlateNumber;


        private void OnEnable()
        {
            navMeshAgent  = GetComponent<NavMeshAgent>();
            originalSpeed = navMeshAgent.speed;
        }

        private void Update()
        {
            DestroyWhenReachedGoal();
        }

        private void DestroyWhenReachedGoal()
        {
            if (!shouldDestroy || navMeshAgent.pathPending ||
                !(navMeshAgent.remainingDistance < turningRadius)) return;

            Destroy(gameObject);
        }

        public void SetShouldDestroy(bool        destroy)      => shouldDestroy = destroy;
        public void SetAgentID(string            status)       => agentID = status;
        public void SetLicensePlateNumber(string number)       => licensePlateNumber = number;
        public void SetSpeed(float               speed)        => navMeshAgent.speed = speed;
        public void SetAngularSpeed(float        angularSpeed) => navMeshAgent.angularSpeed = angularSpeed;
        public void SetAcceleration(float        acceleration) => navMeshAgent.acceleration = acceleration;
        public void SetTurningRadius(float       turnRadius)   => turningRadius = turnRadius;
        public void SetStoppingDistance(float    stopDistance) => navMeshAgent.stoppingDistance = stopDistance;


        public void SetDestination(Vector3 destination)
        {
            if (navMeshAgent != null)
            {
                navMeshAgent.SetDestination(destination);
            }
            else
            {
                Debug.LogError("NavMeshAgent is not assigned.");
            }
        }

        public void TemporaryDeceleration(float interval, float duration, float minSpeed = 0.2f)
        {
            StartCoroutine(TemporaryWaitingRoutine(interval, duration, minSpeed));
        }

        private IEnumerator TemporaryWaitingRoutine(float interval, float duration, float minSpeed)
        {
            yield return new WaitForSeconds(interval);
            SetSpeed(minSpeed);
            yield return new WaitForSeconds(duration);
            SetSpeed(originalSpeed);
        }

        // 觸發事件，通知訂閱者此 AgentEntity 即將銷毀。 (CrowdAgentFactory.cs)
        private void OnDestroy() => onAgentExited?.Invoke(this);
    }
}