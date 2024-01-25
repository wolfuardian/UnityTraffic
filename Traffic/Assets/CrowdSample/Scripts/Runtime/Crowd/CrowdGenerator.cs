using CrowdSample.Scripts.Runtime.Data;
using CrowdSample.Scripts.Utils;
using UnityEngine;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class CrowdGenerator : MonoBehaviour, IUpdatable
    {
        #region Field Declarations

        [SerializeField] private GameObject            pathGo;
        [SerializeField] private GameObject            crowdGo;
        [SerializeField] private CrowdAgentConfig      crowdAgentConfig;
        [SerializeField] private CrowdGenerationConfig crowdGenerationConfig;

        #endregion

        #region Properties

        public GameObject            PathGo                => pathGo;
        public GameObject            CrowdGo               => crowdGo;
        public CrowdAgentConfig      CrowdAgentConfig      => crowdAgentConfig;
        public CrowdGenerationConfig CrowdGenerationConfig => crowdGenerationConfig;

        public bool IsPathGoCreated  => pathGo != null;
        public bool IsCrowdGoCreated => crowdGo != null;
        public bool Initialized      => IsPathGoCreated && IsCrowdGoCreated;

        #endregion

        #region Unity Methods

        //

        #endregion

        #region Public Methods

        public void CreatePathSingleton()
        {
            if (IsPathGoCreated) return;

            var newPath = new GameObject("Path_Root");
            newPath.transform.SetParent(transform);
            newPath.AddComponent<PathBuilder>();

            pathGo = newPath;
        }

        public void CreateCrowdSingleton()
        {
            if (IsCrowdGoCreated) return;

            var newCrowd = new GameObject("Crowd_Root");
            newCrowd.transform.SetParent(transform);

            crowdGo = newCrowd;
        }

        #endregion

        #region Private Methods

        //

        #endregion

        #region Unity Event Methods

        //

        #endregion

        #region Debug and Visualization Methods

        public void UpdateGizmo()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}