using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using CrowdSample.Scripts.Runtime.Data;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AgentEntity : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _navMeshAgent;

        [SerializeField] private bool                         _shouldNotDestroyed;
        [SerializeField] private bool                         _destroyOnNextGoalReached;
        [SerializeField] private CrowdAgentConfig.PermissionID _agentID;
        [SerializeField] private string                       _licensePlateNumber;
        [SerializeField] private float                        _originalSpeed;
        [SerializeField] private float                        _turningRadius;
        [SerializeField] private bool                         _isNaveMeshAgentStale;

        public Action<AgentEntity> OnAgentExited;
        public bool                ShouldNotDestroyed       => _shouldNotDestroyed;
        public bool                DestroyOnNextGoalReached => _destroyOnNextGoalReached;

        public NavMeshAgent                 NavMeshAgent
        {
            get => _navMeshAgent;
            set => _navMeshAgent = value;
        }

        public CrowdAgentConfig.PermissionID AgentID            => _agentID;
        public string                       LicensePlateNumber => _licensePlateNumber;

        private bool IsReachGoal => _navMeshAgent.remainingDistance < _turningRadius;

        private void OnEnable()
        {
            _navMeshAgent  = GetComponent<NavMeshAgent>();
            _originalSpeed = _navMeshAgent.speed;
        }

        private void Update()
        {
            DestroyWhenReachedGoal();
            _isNaveMeshAgentStale = _navMeshAgent.isPathStale;
        }

        private void DestroyWhenReachedGoal()
        {
            if (ShouldNotDestroyed)
            {
                return;
            }

            if (_navMeshAgent.pathPending)
            {
                return;
            }

            if (DestroyOnNextGoalReached && IsReachGoal)
            {
                Destroy(gameObject);
            }
        }


        public void SetShouldNotDestroy(bool                value) => _shouldNotDestroyed = value;
        public void SetDestroyOnNextGoalReached(bool        value) => _destroyOnNextGoalReached = value;
        public void SetAgentID(CrowdAgentConfig.PermissionID value) => _agentID = value;
        public void SetLicensePlateNumber(string            value) => _licensePlateNumber = value;
        public void SetSpeed(float                          value) => _navMeshAgent.speed = value;
        public void SetAngularSpeed(float                   value) => _navMeshAgent.angularSpeed = value;
        public void SetAcceleration(float                   value) => _navMeshAgent.acceleration = value;
        public void SetTurningRadius(float                  value) => _turningRadius = value;
        public void SetStoppingDistance(float               value) => _navMeshAgent.stoppingDistance = value;


        public void SetDestination(Vector3 destination)
        {
            if (_navMeshAgent != null)
            {
                _navMeshAgent.SetDestination(destination);
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
            SetSpeed(_originalSpeed);
        }

        private void OnDestroy() => OnAgentExited?.Invoke(this);
    }
}