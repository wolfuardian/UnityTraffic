using UnityEngine;
using UnityEngine.Events;

namespace CrowdSample.Scripts.Crowd
{
    public class CrowdChecker : MonoBehaviour
    {
        #region Field Declarations

        public UnityEvent<CrowdAgent> onAgentEnter;
        public UnityEvent<CrowdAgent> onAgentExit;
        public UnityEvent<CrowdAgent> onAgentEnterAccess;
        public UnityEvent<CrowdAgent> onAgentEnterDenied;

        #endregion

        #region Unity Methods

        private void OnTriggerEnter(Collider other)
        {
            var agent = other.GetComponent<CrowdAgent>();

            if (agent != null)
            {
                onAgentEnter?.Invoke(agent);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var agent = other.GetComponent<CrowdAgent>();

            if (agent != null)
            {
                onAgentExit?.Invoke(agent);
            }
        }

        #endregion

        #region Unity Event Methods

        public void StartCheckAgent(CrowdAgent agent)
        {
            agent.OnTemporaryHaltAgentBegin += CheckLicensePlate;
            agent.OnTemporaryHaltAgentBegin?.Invoke(agent);
        }

        public void StopCheckAgent(CrowdAgent agent)
        {
            agent.OnTemporaryHaltAgentBegin -= CheckLicensePlate;
            agent.OnTemporaryHaltAgentCompleted?.Invoke(agent);
        }

        private void CheckLicensePlate(CrowdAgent agent)
        {
            if (agent.hasAddonLicensePlate)
            {
                var licensePlate = agent.GetComponent<CrowdAgentAddonLicensePlate>();
                var authState    = licensePlate.licensePlateAuthStates;

                if (authState == LicensePlateStates.Vip || authState == LicensePlateStates.Guest)
                {
                    onAgentEnterAccess?.Invoke(agent);
                }
                else
                {
                    onAgentEnterDenied?.Invoke(agent);
                }
            }
            else
            {
                onAgentEnterDenied?.Invoke(agent);
            }
        }

        #endregion
    }
}