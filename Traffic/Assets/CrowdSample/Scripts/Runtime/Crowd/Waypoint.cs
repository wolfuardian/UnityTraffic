using UnityEditor;
using UnityEngine;
using CrowdSample.Scripts.Utils;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    [ExecuteInEditMode]
    public class Waypoint : MonoBehaviour
    {
        #region Field Declarations

        [SerializeField] private float   radius = 2f;
        [SerializeField] private Vector3 prevPosition;

        #endregion

        #region Properties

        public float Radius => Mathf.Clamp(radius, 0.1f, float.MaxValue);
        public Vector3 PrevPosition
        {
            get => prevPosition;
            set => prevPosition = value;
        }
        
        #endregion

        #region Unity Methods

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            DrawWaypointGizmo();
        }

        private void OnEnable()
        {
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

        private void OnDestroy()
        {
            UpdateCrowdPathController();
            UnityEditorUtils.UpdateAllReceiverImmediately();
        }
#endif

        #endregion

        #region Private Methods

        private void UpdateCrowdPathController()
        {
            var parent = transform.parent;

            if (parent == null) return;

            var targetComponent = parent.GetComponent<CrowdPathController>();

            if (targetComponent == null) return;

            targetComponent.FetchAllNeeded();

            SceneView.RepaintAll();
        }

        #endregion

        #region Debug and Visualization Methods

        private void DrawWaypointGizmo()
        {
            GizmosUtils.Polygon(transform.position, Color.green, Radius, 32);
        }

        #endregion
    }
}