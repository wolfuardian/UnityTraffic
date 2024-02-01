using UnityEngine;
using System.Collections;

namespace CrowdSample.Scripts.Crowd
{
    public class CrowdAgentActionMovement : MonoBehaviour
    {
        #region Field Declarations

        [SerializeField] private float m_waitForStop = 1.0f;
        [SerializeField] private float m_duration    = 1.5f;

        #endregion

        #region Properties

        public float waitForStop => m_waitForStop;

        public float duration => m_duration;

        #endregion

        #region Public Methods

        public void Stopping(CrowdAgent agent)
        {
            StartCoroutine(TemporaryHaltRoutine(agent));
        }

        #endregion

        #region Private Methods

        private IEnumerator TemporaryHaltRoutine(CrowdAgent agent)
        {
            yield return new WaitForSeconds(waitForStop);
            StoppingAgentSpeed(agent);
            yield return new WaitForSeconds(duration);
            ResumeAgentSpeed(agent);
        }

        private void StoppingAgentSpeed(CrowdAgent agent)
        {
            agent.originalSpeed      = agent.navMeshAgent.speed;
            agent.navMeshAgent.speed = 0;
        }

        private void ResumeAgentSpeed(CrowdAgent agent)
        {
            agent.navMeshAgent.speed = agent.originalSpeed;
        }

        #endregion
    }
}