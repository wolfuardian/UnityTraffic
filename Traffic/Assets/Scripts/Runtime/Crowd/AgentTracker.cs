using UnityEngine;
using System.Collections.Generic;

namespace Runtime.Crowd
{
    public class AgentTracker : MonoBehaviour
    {
        public CrowdPath crowdPath;
        public AgentEntity agent;
        public GameObject previousTarget;
        public GameObject target;
        public int targetIndex;
        public List<GameObject> targets;
        [Range(0.0f, 1.0f)] public float globalJourney;
        [Range(0.0f, 1.0f)] public float localJourney;
        public float localDistance;
        public float remainingDistance;


        public LineRenderer lineRenderer;
        private int m_TargetIndex;


        private void Start()
        {
            previousTarget = gameObject;
            targets = crowdPath.points;
            target = targets[0];
            m_TargetIndex = 0;

            InitialGuideline();
            SetAgentDestination();
        }

        private void Update()
        {
            UpdatingTarget();

            if (previousTarget != null)
            {
                localDistance = Vector3.Distance(previousTarget.transform.position, target.transform.position);
            }

            targetIndex = targets.IndexOf(target);
            remainingDistance = agent.navMeshAgent.remainingDistance;
            localJourney = Mathf.Clamp(1 - remainingDistance / localDistance, 0f, 1f);
            globalJourney = (targetIndex + localJourney) / targets.Count;

            UpdatingGuideline();
        }

        private void UpdatingTarget()
        {
            if (Vector3.Distance(transform.position, target.transform.position) < 2f)
            {
                FindNextTarget();
            }

            SetAgentDestination();
        }

        private void FindNextTarget()
        {
            previousTarget = target;

            var nextIndex = crowdPath.isLooping ? (m_TargetIndex + 1) % targets.Count : m_TargetIndex + 1;

            if (nextIndex >= targets.Count) return;

            m_TargetIndex = nextIndex;
            target = targets[m_TargetIndex];
        }

        private void SetAgentDestination()
        {
            agent.SetDestination(target.transform.position);
        }


        private void InitialGuideline()
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
            lineRenderer.positionCount = 2;
        }

        private void UpdatingGuideline()
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, target.transform.position);
        }
    }
}