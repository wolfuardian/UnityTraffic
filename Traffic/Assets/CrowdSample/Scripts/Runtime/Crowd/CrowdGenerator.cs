using UnityEngine;
using CrowdSample.Scripts.Runtime.Data;
using CrowdSample.Scripts.Utils;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class CrowdGenerator : MonoBehaviour, IUpdateReceiver
    {
        #region Field Declarations

        [SerializeField] private CrowdPath             m_path;
        [SerializeField] private CrowdSpawner          m_spawner;
        [SerializeField] private CrowdGenerationConfig m_crowdGenerationConfig;

        #endregion

        #region Properties

        public CrowdGenerationConfig crowdGenerationConfig => m_crowdGenerationConfig;

        public bool createdPath    => m_path != null;
        public bool createdSpawner => m_spawner != null;
        public bool initialized    => createdPath && createdSpawner;

        #endregion

        #region Implementation Methods

        public void UpdateImmediately()
        {
            if (createdPath)
            {
                m_path.crowdGenerationConfig = m_crowdGenerationConfig;
                m_path.ApplyPresetProperties();
            }

            if (createdSpawner)
            {
                m_spawner.crowdGenerationConfig = m_crowdGenerationConfig;
                m_spawner.ApplyPresetProperties();
            }
        }

        #endregion

        #region Unity Methods

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (crowdGenerationConfig == null) return;
            UnityEditorUtils.UpdateAllReceiverImmediately();
        }
#endif

        #endregion

        #region Public Methods

        public void CreatePathInstance()
        {
            if (createdPath) return;

            var instance  = new GameObject("_Path");
            var component = instance.AddComponent<CrowdPath>();

            instance.transform.SetParent(transform);
            instance.AddComponent<CrowdPathBuilder>();

            m_path = component;
        }

        public void CreateSpawnerInstance()
        {
            if (createdSpawner) return;

            var instance  = new GameObject("_Spawner");
            var component = instance.AddComponent<CrowdSpawner>();

            instance.transform.SetParent(transform);

            m_spawner = component;
        }

        #endregion
    }
}