using UnityEngine;

namespace CrowdSample.Scripts.Runtime.Agent
{
    public class Waypoint : MonoBehaviour
    {
        [SerializeField] private float radius = 2f;

        public float Radius => radius;

        public void SetRadius(float value) => radius = value;

        private void Start()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }

        private void OnValidate()
        {
            SetRadius(Mathf.Clamp(Radius, 0.1f, float.MaxValue));
        }
    }
}