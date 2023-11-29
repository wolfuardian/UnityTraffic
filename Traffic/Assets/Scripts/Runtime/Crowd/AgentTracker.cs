using System;
using UnityEngine;
using System.Collections.Generic;

namespace Runtime.Crowd
{
    public class AgentTracker : MonoBehaviour
    {
        public AgentEntity agent;
        public GameObject previousTarget;
        public GameObject target;
        public int targetIndex;
        private List<GameObject> _targets;
        [SerializeField] [Range(0.0f, 1.0f)] private float globalJourney;
        [SerializeField] [Range(0.0f, 1.0f)] private float localJourney;
        public float localDistance;
        public float remainingDistance;
        public int maxTargetIndex;
        private CrowdPath _crowdPath;
        private float _turningRadius;

        public bool isGoingForward = true;
        public bool isDestroyOnArrival = true;


        public LineRenderer lineRenderer;

        public AgentTracker(List<GameObject> targets)
        {
            _targets = targets;
        }

        private void Start()
        {
            previousTarget = gameObject;
            _targets = _crowdPath.waypoints;
            target = _targets[0];
            targetIndex = 0;
            localDistance = Vector3.Distance(previousTarget.transform.position, target.transform.position);
            maxTargetIndex = _targets.Count - 1;

            InitialGuideline();
            SetAgentDestination();
        }

        private void Update()
        {
            MoveToward();

            CalculateJourney();

            RedrawGuide();
        }

        public void SetAgentEntity(AgentEntity agentEntity) => agent = agentEntity;
        public void SetCrowdPath(CrowdPath crowdPath) => _crowdPath = crowdPath;
        public void SetTurningRadius(float turningRadius) => _turningRadius = turningRadius;


        private void CalculateJourney()
        {
            targetIndex = _targets.IndexOf(target);
            remainingDistance = agent.navMeshAgent.remainingDistance;
            localDistance = Vector3.Distance(previousTarget.transform.position, target.transform.position);
            localJourney = Mathf.Clamp(1 - remainingDistance / localDistance, 0f, 1f);
            if (previousTarget == target) localJourney = 1f;

            globalJourney = (targetIndex + localJourney) / _targets.Count;

            if (Math.Abs(globalJourney - 1f) < 0.001f)
            {
                agent.shouldDestroy = true;
            }
        }

        private float CalculatePingPongJourneyIndex()
        {
            var effectiveIndex = isGoingForward ? targetIndex : maxTargetIndex - targetIndex;
            return (float)effectiveIndex / maxTargetIndex;
        }

        private void MoveToward()
        {
            if (Vector3.Distance(transform.position, target.transform.position) < _turningRadius)
            {
                previousTarget = target;
                if (targetIndex < _targets.Count - 1)
                {
                    targetIndex++;
                }

                if (targetIndex < _targets.Count && targetIndex >= 0)
                {
                    target = _targets[targetIndex];
                }
            }

            SetAgentDestination();
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

        private void RedrawGuide()
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, target.transform.position);
        }
    }
}