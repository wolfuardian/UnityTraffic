using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace CrowdSample.Scripts.Crowd
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class CrowdAgent : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent m_navMeshAgent;
        [SerializeField] private Rigidbody    m_rigidBody;
        [SerializeField] private float        m_originalSpeed;

        [SerializeField] private bool m_hasAddonLicensePlate;

        public UnityAction<CrowdAgent> AgentExited;

        public UnityAction<CrowdAgent> OnTemporaryHaltAgentBegin;
        public UnityAction<CrowdAgent> OnTemporaryHaltAgentCompleted;

        private List<Color> originalColors = new List<Color>();

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

        public bool hasAddonLicensePlate
        {
            get => m_hasAddonLicensePlate;
            set => m_hasAddonLicensePlate = value;
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

        private void OnEnable()
        {
            OnTemporaryHaltAgentBegin     += ChangeColorToRed;
            OnTemporaryHaltAgentCompleted += ChangeColorToOriginal;
        }

        private void OnDisable()
        {
            OnTemporaryHaltAgentBegin     -= ChangeColorToRed;
            OnTemporaryHaltAgentCompleted -= ChangeColorToOriginal;
        }

        private void Start()
        {
            TemporaryDeceleration(0, 0.5f);
            hasAddonLicensePlate = GetComponent<CrowdAgentAddonLicensePlate>() != null;
        }

        #endregion

        public void ChangeColorToRed(CrowdAgent agent)
        {
            var meshRenderers = GetComponentsInChildren<MeshRenderer>();
            if (meshRenderers.Length == 0) return;
            meshRenderers.ToList().ForEach(meshRenderer =>
            {
                originalColors.Add(meshRenderer.material.color);
                meshRenderer.material.color = Color.red;
            });
        }

        public void ChangeColorToOriginal(CrowdAgent agent)
        {
            var meshRenderers = GetComponentsInChildren<MeshRenderer>();
            if (meshRenderers.Length == 0) return;
            meshRenderers.ToList().ForEach(meshRenderer =>
            {
                if (originalColors.Count == 0) return;
                meshRenderer.material.color = originalColors[meshRenderers.ToList().IndexOf(meshRenderer)];
            });
            originalColors.Clear();
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

        private void OnDestroy() => AgentExited?.Invoke(this);
    }
}