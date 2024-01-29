using UnityEditor;
using UnityEngine;
using CrowdSample.Scripts.Utils;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    [ExecuteInEditMode]
    public class Waypoint : MonoBehaviour
    {
        #region Field Declarations

        [SerializeField] private float m_radius = 2f;

        #endregion

        #region Properties

        public float   radius       => Mathf.Clamp(m_radius, 0.1f, float.MaxValue);
        public Vector3 prevPosition { get; set; }

        #endregion

        #region Unity Methods

#if UNITY_EDITOR

        private void OnEnable()
        {
            UnityUtils.UpdateAllReceiverImmediately();

            UpdateCrowdPathController();
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
            UpdateCrowdPathController();
            UnityUtils.UpdateAllReceiverImmediately();
        }
#endif

        #endregion

        #region Private Methods

        private void UpdateCrowdPathController()
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
            GizmosUtils.Polygon(transform.position, Color.green, radius, 32);
        }

        #endregion
    }
}