using UnityEditor;
using UnityEngine;
using CrowdSample.Scripts.Utils;
using CrowdSample.Scripts.Runtime.Crowd;

namespace CrowdSample.Scripts.Editor.Crowd
{
    [CustomEditor(typeof(WaypointGizmos))]
    public class WaypointGizmosEditor : UnityEditor.Editor
    {
        private Vector3 previousPosition;

        private void OnSceneGUI()
        {
            var waypointGizmos = (WaypointGizmos)target;

            DetectWaypointPositionChange(waypointGizmos);
        }

        private void DetectWaypointPositionChange(WaypointGizmos waypointGizmos)
        {
            if (waypointGizmos.transform.position != previousPosition)
            {
                previousPosition = waypointGizmos.transform.position;
                UpdateGizmos();
            }
        }

        private void UpdateGizmos()
        {
            UnityEditorUtils.UpdateAllGizmos();
            SceneView.RepaintAll();
        }
    }
}