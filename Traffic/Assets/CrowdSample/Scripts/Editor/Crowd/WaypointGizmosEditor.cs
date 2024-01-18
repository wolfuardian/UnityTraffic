using UnityEditor;
using UnityEngine;
using CrowdSample.Scripts.Utils;
using CrowdSample.Scripts.Runtime.Agent;

namespace CrowdSample.Scripts.Editor.Crowd
{
    [CustomEditor(typeof(WaypointGizmos))]
    public class WaypointGizmosEditor : UnityEditor.Editor
    {
        private Vector3 previousPosition;

        private void OnSceneGUI()
        {
            var waypointGizmos = (WaypointGizmos)target;
            if (waypointGizmos.transform.position == previousPosition) return;
            previousPosition = waypointGizmos.transform.position;

            UnityEditorUtils.UpdateAllGizmos();
        }
    }
}