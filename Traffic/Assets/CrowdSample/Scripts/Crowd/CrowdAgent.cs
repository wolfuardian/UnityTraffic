using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace CrowdSample.Scripts.Crowd
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class CrowdAgent : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent m_navMeshAgent;
        [SerializeField] private Rigidbody    m_rigidBody;
        [SerializeField] private float        m_originalSpeed;

        [SerializeField] private PermissionStates m_permissionStates;

        [SerializeField] private string m_userIdentity;
        [SerializeField] private string m_userType;

        public Action<CrowdAgent> AgentExited;


        public NavMeshAgent navMeshAgent
        {
            get => m_navMeshAgent;
            set => m_navMeshAgent = value;
        }

        public Rigidbody rigidBody
        {
            get => m_rigidBody;
            set => m_rigidBody = value;
        }

        public float originalSpeed
        {
            get => m_originalSpeed;
            set => m_originalSpeed = value;
        }

        public PermissionStates permissionStates
        {
            get => m_permissionStates;
            set => m_permissionStates = value;
        }

        public string userIdentity
        {
            get => m_userIdentity;
            set => m_userIdentity = value;
        }

        public string userType
        {
            get => m_userType;
            set => m_userType = value;
        }

        #region Unity Methods

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>() == null
                ? gameObject.AddComponent<NavMeshAgent>()
                : GetComponent<NavMeshAgent>();
            rigidBody = GetComponent<Rigidbody>() == null
                ? gameObject.AddComponent<Rigidbody>()
                : GetComponent<Rigidbody>();
        }

        private void Start()
        {
            TemporaryDeceleration(0, 0.5f);
        }

        #endregion


        public void TemporaryDeceleration(float interval, float duration, float minSpeed = 0.2f)
        {
            originalSpeed = navMeshAgent.speed;
            StartCoroutine(TemporaryWaitingRoutine(interval, duration));
        }

        private IEnumerator TemporaryWaitingRoutine(float interval, float duration)
        {
            yield return new WaitForSeconds(interval);
            SetSpeed(0);
            yield return new WaitForSeconds(duration);
            SetSpeed(originalSpeed);
        }

        private void SetSpeed(float speed)
        {
            navMeshAgent.speed = speed;
        }

        private void OnDestroy() => AgentExited?.Invoke(this);
    }
}