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
        [SerializeField] private float        m_originalSpeed;

        [SerializeField] private int    m_agentID;
        [SerializeField] private string m_type     = "No Data";
        [SerializeField] private string m_category = "No Data";
        [SerializeField] private string m_alias    = "No Data";
        [SerializeField] private string m_model    = "No Data";
        [SerializeField] private string m_time     = "No Data";
        [SerializeField] private string m_noted    = "No Data";

        public Action<CrowdAgent> AgentExited;


        public NavMeshAgent navMeshAgent
        {
            get => m_navMeshAgent;
            set => m_navMeshAgent = value;
        }

        public float originalSpeed
        {
            get => m_originalSpeed;
            set => m_originalSpeed = value;
        }


        public int agentID
        {
            get => m_agentID;
            set => m_agentID = value;
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
            navMeshAgent = GetComponent<NavMeshAgent>() ?? gameObject.AddComponent<NavMeshAgent>();
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