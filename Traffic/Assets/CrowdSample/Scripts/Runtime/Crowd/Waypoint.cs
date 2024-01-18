using UnityEngine;
using CrowdSample.Scripts.Utils;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class Waypoint : MonoBehaviour
    {
        public float radius = 2f;

        private void OnDrawGizmos()
        {
            var position = transform.position;
            GizmosUtils.Polygon(position, Color.green, radius, 32);
        }

        private void Start()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}