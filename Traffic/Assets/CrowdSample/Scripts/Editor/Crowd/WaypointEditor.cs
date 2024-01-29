using UnityEditor;
using CrowdSample.Scripts.Utils;
using CrowdSample.Scripts.Runtime.Crowd;

namespace CrowdSample.Scripts.Editor.Crowd
{
    [CustomEditor(typeof(Waypoint))]
    public class WaypointEditor : UnityEditor.Editor
    {
        #region Field Declarations

        private Waypoint waypoint;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            waypoint = (Waypoint)target;
        }

        private void OnSceneGUI()
        {
            if (waypoint.transform.position == waypoint.prevPosition) return;

            waypoint.prevPosition = waypoint.transform.position;

            UnityUtils.UpdateAllReceiverImmediately();

            SceneView.RepaintAll();
        }

        #endregion
    }
}