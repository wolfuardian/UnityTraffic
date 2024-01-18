using UnityEngine;
using CrowdSample.Scripts.Utils;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class Waypoint : MonoBehaviour
    {
        [SerializeField] private float radius = 2f;

        public float Radius => radius;

        public void SetRadius(float value) => radius = value;

        private void OnDrawGizmos()
        {
            var position = transform.position;
            GizmosUtils.Polygon(position, Color.green, Radius, 32);
        }

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