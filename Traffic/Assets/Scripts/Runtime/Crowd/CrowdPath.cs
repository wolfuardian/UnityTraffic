// PathBuilder.cs

using System;
using UnityEngine;
using System.Collections.Generic;

namespace Runtime.Crowd
{
    [ExecuteInEditMode]
    public class CrowdPath : MonoBehaviour
    {
        [NonSerialized] public bool isInEditMode;
        public List<GameObject> waypoints = new List<GameObject>();

        void OnDrawGizmos()
        {
            if (waypoints.Count > 1)
            {
                Gizmos.color = Color.cyan;
                
                for (int i = 0; i < waypoints.Count - 1; i++)
                {
                    if (waypoints[i] != null && waypoints[i + 1] != null)
                    {
                        Gizmos.DrawLine(waypoints[i].transform.position, waypoints[i + 1].transform.position);
                    }
                }
            }
        }

        public void AddPoint(Vector3 position)
        {
            if (!isInEditMode) return;

            var point = new GameObject("Point" + waypoints.Count)
            {
                transform = { position = position }
            };
            point.transform.SetParent(transform);

            var meshFilter = point.AddComponent<MeshFilter>();
            var meshRenderer = point.AddComponent<MeshRenderer>();
            meshFilter.mesh = CreateSphereMesh();
            meshRenderer.material = new Material(Shader.Find("Standard"));

            point.AddComponent<CrowdPathPoint>();

            waypoints.Add(point);
            SortTargetPoints();
        }

        private static Mesh CreateSphereMesh()
        {
            return Resources.GetBuiltinResource<Mesh>("New-Sphere.fbx");
        }

        private void SortTargetPoints()
        {
            var pointsCount = waypoints.Count;
            foreach (var point in waypoints)
            {
                point.GetComponent<CrowdPathPoint>().pointIndex = waypoints.IndexOf(point);
                point.GetComponent<CrowdPathPoint>().isLastPoint =
                    point.GetComponent<CrowdPathPoint>().pointIndex == pointsCount - 1;
            }
        }
    }
}