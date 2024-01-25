using UnityEngine;
using CrowdSample.Scripts.Runtime.Data;
using CrowdSample.Scripts.Utils;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class CrowdGenerator : MonoBehaviour, IUpdateReceiver
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
        public bool IsInitialized         => IsPathGoCreated && IsCrowdAgentGoCreated;

        #endregion

        #region Unity Methods

#if UNITY_EDITOR
        private void OnValidate()
        {
            FetchCrowdGenerationConfigs();
            UnityEditorUtils.UpdateAllReceiverImmediately();
        }
#endif

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
            newCrowdAgentGo.AddComponent<CrowdFactoryController>();

            crowdAgentGo = newCrowdAgentGo;
        }

        #endregion

        #region Private Methods

        private void FetchCrowdGenerationConfigs()
        {
            if (CrowdGenerationConfig == null) return;
            ApplyGenerationConfigSettings();
        }

        private void ApplyGenerationConfigSettings()
        {
            var pathController = pathGo.GetComponent<CrowdPathController>();
            pathController.IsSpawnAgentOnce = CrowdGenerationConfig.IsSpawnAgentOnce;
            pathController.IsClosedPath     = CrowdGenerationConfig.IsClosedPath;
            pathController.IsUseSpacing     = CrowdGenerationConfig.IsUseSpacing;
            pathController.Count            = CrowdGenerationConfig.InstantCount;
            pathController.CountMax         = CrowdGenerationConfig.MaxCount;
            pathController.Offset           = CrowdGenerationConfig.Offset;
            pathController.Spacing          = CrowdGenerationConfig.Spacing;
        }

        #endregion

        #region Implementation Methods

        public void UpdateImmediately()
        {
            FetchCrowdGenerationConfigs();
        }

        #endregion
    }
}