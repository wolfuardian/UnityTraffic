// using System;
// using UnityEngine;
//
// namespace Runtime.Crowd
// {
//     public class CrowdPathGuide : MonoBehaviour
//     {
//         public GameObject target;
//         public bool showGuideInEditor = true;
//         public LineRenderer lineRenderer;
//
//         public void InitialGuideline()
//         {
//             lineRenderer.startWidth = 0.1f;
//             lineRenderer.endWidth = 0.1f;
//             lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
//             lineRenderer.startColor = Color.red;
//             lineRenderer.endColor = Color.red;
//             lineRenderer.positionCount = 2;
//         }
//
//         public void RedrawGuide()
//         {
//             var targetPosition = target != null ? target.transform.position : Vector3.zero;
//             lineRenderer.SetPosition(0, transform.position);
//             lineRenderer.SetPosition(1, targetPosition);
//         }
//     }
// }