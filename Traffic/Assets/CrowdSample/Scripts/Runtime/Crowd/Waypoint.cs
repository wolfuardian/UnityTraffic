using UnityEditor;
using UnityEngine;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    [ExecuteInEditMode]
    public class Waypoint : MonoBehaviour
    {
        [SerializeField] private float radius = 2f;

        public float Radius => Mathf.Clamp(radius, 0.1f, float.MaxValue);

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateRadius();
        }

        private void OnEnable()
        {
            UpdatePathConfiguration();
        }

        private void OnDestroy()
        {
            UpdatePathConfiguration();
        }

        private void UpdateRadius()
        {
            radius = Radius;
        }

        private void UpdatePathConfiguration()
        {
            var parent = transform.parent;
            if (parent == null) return;

            var path = parent.GetComponent<Path>();
            if (path == null) return;

            path.UpdatePathConfiguration();
            SceneView.RepaintAll();
        }
#endif

        private void Start()
        {
            ToggleMeshRenderer(false);
        }

        private void ToggleMeshRenderer(bool isEnabled)
        {
            var meshRenderer = gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = isEnabled;
            }
        }
    }
}