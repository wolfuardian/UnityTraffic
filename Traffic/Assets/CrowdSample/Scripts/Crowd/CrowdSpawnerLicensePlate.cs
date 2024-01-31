using UnityEngine;

namespace CrowdSample.Scripts.Crowd
{
    public class CrowdSpawnerLicensePlate : MonoBehaviour
    {
        #region Field Declarations

        [SerializeField] private CrowdSpawner         m_crowdSpawner;
        [SerializeField] private string               m_licensePlateCsvPath;
        private                  LicensePlateRegistry licensePlateRegistry;

        #endregion

        #region Properties

        public CrowdSpawner crowdSpawner
        {
            get => m_crowdSpawner;
            set => m_crowdSpawner = value;
        }

        public string licensePlateCsvPath
        {
            get => m_licensePlateCsvPath;
            set => m_licensePlateCsvPath = value;
        }

        #endregion

        #region Unity Methods

        private void Start()
        {
            if (licensePlateRegistry == null) return;
            licensePlateRegistry = new LicensePlateRegistry(licensePlateCsvPath);
        }

        #endregion


        #region Private Methods

        #endregion
    }
}