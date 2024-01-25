using UnityEngine;
using CrowdSample.Scripts.Runtime.Data;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class CrowdGenerator : MonoBehaviour
    {
        #region Field Declarations

        [SerializeField] private GameObject            pathGo;
        [SerializeField] private GameObject            crowdAgentGo;
        [SerializeField] private CrowdAgentConfig      crowdAgentConfig;
        [SerializeField] private CrowdGenerationConfig crowdGenerationConfig;

        #endregion

        #region Properties

        public GameObject PathGo
        {
            get => pathGo;
            set => pathGo = value;
        }

        public GameObject CrowdAgentGo
        {
            get => crowdAgentGo;
            set => crowdAgentGo = value;
        }

        public CrowdAgentConfig      CrowdAgentConfig      => crowdAgentConfig;
        public CrowdGenerationConfig CrowdGenerationConfig => crowdGenerationConfig;

        public bool IsPathGoCreated       => pathGo != null;
        public bool IsCrowdAgentGoCreated => crowdAgentGo != null;
        public bool IsInitialized           => IsPathGoCreated && IsCrowdAgentGoCreated;

        #endregion

        #region Public Methods

        public void CreatePathGo()
        {
            if (IsPathGoCreated) return;

            var newPathGo = new GameObject("_Path");
            newPathGo.transform.SetParent(transform);
            newPathGo.AddComponent<CrowdPathBuilder>();

            pathGo = newPathGo;
        }

        public void CreateCrowdAgentGo()
        {
            if (IsCrowdAgentGoCreated) return;

            var newCrowdAgentGo = new GameObject("_CrowdAgent");
            newCrowdAgentGo.transform.SetParent(transform);

            crowdAgentGo = newCrowdAgentGo;
        }

        #endregion
    }
}