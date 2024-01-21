using UnityEditor;
using UnityEngine;
using CrowdSample.Scripts.Utils;
using CrowdSample.Scripts.Runtime.Crowd;

namespace CrowdSample.Scripts.Editor.Crowd
{
    [CustomEditor(typeof(WaypointGizmos))]
    public class WaypointGizmosEditor : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            var waypointGizmos = (WaypointGizmos)target;

            DetectWaypointPositionChange(waypointGizmos);
        }

        private void DetectWaypointPositionChange(WaypointGizmos waypointGizmos)
        {
            if (waypointGizmos.transform.position != waypointGizmos.PrevPosition)
            {
                waypointGizmos.PrevPosition = waypointGizmos.transform.position;
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