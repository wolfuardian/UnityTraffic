using UnityEngine;

namespace Runtime.Crowd
{
    public class CrowdPathPoint : MonoBehaviour
    {
        public float colliderRadius = 2f;
        public int pointIndex;
        public bool isLastPoint;
        public float allowableRadius = 2f;


        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            const int segments = 360;

            var radius = allowableRadius;

            var previousPoint = transform.position + new Vector3(radius, 0, 0);

            for (var i = 1; i <= segments; i++)
            {
                var angle = (float)i / segments * 2 * Mathf.PI;

                var currentPoint = transform.position +
                                   new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

                Gizmos.DrawLine(previousPoint, currentPoint);

                previousPoint = currentPoint;
            }
        }

        private void Start()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}