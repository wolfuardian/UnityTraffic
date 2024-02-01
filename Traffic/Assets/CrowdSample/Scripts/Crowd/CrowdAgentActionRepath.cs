using UnityEngine;

namespace CrowdSample.Scripts.Crowd
{
    public class CrowdAgentActionRepath : MonoBehaviour
    {
        #region Field Declarations

        [SerializeField] private CrowdPath m_gotoPath;
        [SerializeField] private int       m_targetWaypointID;

        #endregion

        #region Properties

        public CrowdPath gotoPath => m_gotoPath;

        public int targetWaypointID => m_targetWaypointID;

        #endregion

        #region Public Methods

        public void To(CrowdAgent agent)
        {
            if (gotoPath == null) return;
            var navigator = agent.GetComponent<CrowdNavigator>();
            if (navigator == null) return;
            navigator.crowdPath     = gotoPath;
            navigator.waypoints     = gotoPath.waypoints;
            navigator.targetPointID = targetWaypointID;
        }

        #endregion
    }
}