using UnityEngine;
using CrowdSample.Scripts.Runtime.Data;
using CrowdSample.Scripts.Utils;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class CrowdGenerator : MonoBehaviour, IUpdateReceiver
    {
        #region Field Declarations

        [SerializeField] private CrowdPath    m_path;
        [SerializeField] private CrowdSpawner m_spawner;
        [SerializeField] private CrowdConfig  m_crowdConfig;

        #endregion

        #region Properties

        public CrowdConfig crowdConfig    => m_crowdConfig;
        public bool        createdPath    => m_path != null;
        public bool        createdSpawner => m_spawner != null;
        public bool        initialized    => createdPath && createdSpawner;

        #endregion

#if UNITY_EDITOR

        #region Implementation Methods

        public void UpdateImmediately()
        {
            if (createdPath)
                
            {
                m_path.crowdConfig = m_crowdConfig;
                // m_path.FetchAll();
            }

            if (createdSpawner)
            {
                m_spawner.path = m_path;
                m_spawner.ApplyPresetProperties();
            }
        }

        #endregion

        #region Unity Methods

        private void OnValidate()
        {
            if (crowdConfig == null) return;
            UnityUtils.UpdateAllReceiverImmediately();
        }

        #endregion

#endif

        #region Public Methods

        public void CreatePathInstance()
        {
            if (createdPath) return;

            var instance  = new GameObject("_Path");
            var component = instance.AddComponent<CrowdPath>();

            component.crowdConfig = m_crowdConfig;

            instance.transform.SetParent(transform);
            instance.AddComponent<CrowdPath>();

            m_path = component;
        }

        public void CreateSpawnerInstance()
        {
            if (createdSpawner) return;

            var instance  = new GameObject("_Spawner");
            var component = instance.AddComponent<CrowdSpawner>();

            component.path = m_path;

            instance.transform.SetParent(transform);

            m_spawner = component;
        }

        #endregion
    }
}