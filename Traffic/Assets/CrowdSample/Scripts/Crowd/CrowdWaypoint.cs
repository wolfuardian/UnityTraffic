using UnityEditor;
using UnityEngine;

namespace CrowdSample.Scripts.Crowd
{
    [ExecuteInEditMode]
    public class CrowdWaypoint : MonoBehaviour
    {
        #region Field Declarations

        [SerializeField] private float m_radius = 2f;

        #endregion

        #region Properties

        public float radius => m_radius;

        public int waypointID { get; set; }

        public Vector3 prevPosition { get; set; }

        #endregion

        #region Unity Methods

#if UNITY_EDITOR

        private void OnEnable()
        {
            CrowdUtils.UpdateAllReceiverImmediately();

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
            CrowdUtils.UpdateAllReceiverImmediately();
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
                fontSize  = 13
            };

            var text = "" +
                       "\n" +
                       "  PtNum: " + waypointID;
            
            Handles.Label(position, text, style);
        }

        #endregion
    }


    [CustomEditor(typeof(CrowdWaypoint))]
    public class WaypointEditor : Editor
    {
        #region Field Declarations

        private CrowdWaypoint crowdWaypoint;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            crowdWaypoint = (CrowdWaypoint)target;
        }

        private void OnSceneGUI()
        {
            if (crowdWaypoint.transform.position == crowdWaypoint.prevPosition) return;

            crowdWaypoint.prevPosition = crowdWaypoint.transform.position;

            CrowdUtils.UpdateAllReceiverImmediately();

            SceneView.RepaintAll();
        }

        #endregion
    }
}