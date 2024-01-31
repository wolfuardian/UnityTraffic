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

        [SerializeField] private int    m_entityID;
        [SerializeField] private string m_type     = "No Data";
        [SerializeField] private string m_category = "No Data";
        [SerializeField] private string m_alias    = "No Data";
        [SerializeField] private string m_model    = "No Data";
        [SerializeField] private string m_time     = "No Data";
        [SerializeField] private string m_noted    = "No Data";

        public Action<CrowdAgent> EntityExited;


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

        public int entityID
        {
            get => m_entityID;
            set => m_entityID = value;
        }

        public string type
        {
            get => m_type;
            set => m_type = value;
        }

        public string category
        {
            get => m_category;
            set => m_category = value;
        }

        public string alias
        {
            get => m_alias;
            set => m_alias = value;
        }

        public string model
        {
            get => m_model;
            set => m_model = value;
        }

        public string time
        {
            get => m_time;
            set => m_time = value;
        }

        public string noted
        {
            get => m_noted;
            set => m_noted = value;
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

        private void Update()
        {
        }


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

        private void OnDestroy() => EntityExited?.Invoke(this);
    }
}