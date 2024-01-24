using CrowdSample.Scripts.Utils;
using UnityEngine;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class CrowdGenerator : MonoBehaviour, IUpdatable
    {
        #region Field Declarations

        [SerializeField] private GameObject pathSingleton;
        [SerializeField] private GameObject crowdSingleton;

        #endregion

        #region Properties

        public bool IsPathCreated  => pathSingleton != null;
        public bool IsCrowdCreated => crowdSingleton != null;
        public bool Initialized    => IsPathCreated && IsCrowdCreated;

        #endregion

        #region Unity Methods

        //

        #endregion

        #region Public Methods

        public void CreatePathSingleton()
        {
            if (IsPathCreated) return;

            var newPath = new GameObject("Path");
            newPath.transform.SetParent(transform);

            pathSingleton = newPath;
        }

        public void CreateCrowdSingleton()
        {
            if (IsCrowdCreated) return;

            var newCrowd = new GameObject("Crowd");
            newCrowd.transform.SetParent(transform);

            crowdSingleton = newCrowd;
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