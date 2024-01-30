using UnityEditor;
using UnityEngine;
using CrowdSample.Scripts.Utils;

namespace CrowdSample.Scripts.Crowd
{
    [ExecuteInEditMode]
    public class Waypoint : MonoBehaviour
    {
        #region Field Declarations

        [SerializeField] private float   m_radius = 2f;
        [SerializeField] private int     m_index;
        [SerializeField] private Vector3 m_prevPosition;

        #endregion

        #region Properties

        public float radius => m_radius;

        public int index
        {
            get => m_index;
            set => m_index = value;
        }

        public Vector3 prevPosition
        {
            get => m_prevPosition;
            set => m_prevPosition = value;
        }

        #endregion

        #region Unity Methods

#if UNITY_EDITOR

        private void OnEnable()
        {
            UnityUtils.UpdateAllReceiverImmediately();

            UpdateCrowdPath();
        }

        private void Start()
        {
            var meshRenderer = gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false;
            }
        }

        private void OnDrawGizmos()
        {
            DrawWaypointGizmo();
        }

        private void OnDestroy()
        {
            UpdateCrowdPath();
            UnityUtils.UpdateAllReceiverImmediately();
        }
#endif

        #endregion

        #region Private Methods

        private void UpdateCrowdPath()
        {
            var parent = transform.parent;

            if (parent == null) return;

            var targetComponent = parent.GetComponent<CrowdPath>();

            if (targetComponent == null) return;

            targetComponent.UpdateImmediately();

            SceneView.RepaintAll();
        }

        #endregion

        #region Debug and Visualization Methods

        private void DrawWaypointGizmo()
        {
            var position = transform.position;

            GizmosUtils.Polygon(position, Color.green, radius, 16);

            var style = new GUIStyle
            {
                normal    = { textColor = Color.cyan },
                alignment = TextAnchor.UpperLeft,
                fontSize  = 9
            };

            Handles.Label(position, "PointNum: " + index, style);
        }

        #endregion
    }


    [CustomEditor(typeof(Waypoint))]
    public class WaypointEditor : Editor
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