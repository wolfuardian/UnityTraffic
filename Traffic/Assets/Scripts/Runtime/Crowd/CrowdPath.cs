// PathBuilder.cs

using System;
using UnityEngine;
using System.Collections.Generic;

namespace Runtime.Crowd
{
    [ExecuteInEditMode]
    public class CrowdPath : MonoBehaviour
    {
        [NonSerialized] public bool IsInEditMode;
        public List<GameObject> points = new List<GameObject>();
        public bool isLooping = true;
        public bool hasGoal = false;

        public enum PathMode
        {
            Loop,
            OneWay,
            PingPong
        };

        public PathMode pathMode = PathMode.Loop;
        public float agentSpeed = 10f;
        private int m_PointCounter;

        public void AddPoint(Vector3 position)
        {
            if (!IsInEditMode) return;

            var point = new GameObject("Point" + m_PointCounter++)
            {
                transform = { position = position }
            };
            point.transform.SetParent(transform);

            var meshFilter = point.AddComponent<MeshFilter>();
            var meshRenderer = point.AddComponent<MeshRenderer>();
            meshFilter.mesh = CreateSphereMesh();
            meshRenderer.material = new Material(Shader.Find("Standard"));

            point.AddComponent<TargetPoint>();

            points.Add(point);
            SortTargetPoints();
        }

        private static Mesh CreateSphereMesh()
        {
            return Resources.GetBuiltinResource<Mesh>("New-Sphere.fbx");
        }

        private void SortTargetPoints()
        {
            var pointsCount = points.Count;
            foreach (var point in points)
            {
                point.GetComponent<TargetPoint>().pointIndex = points.IndexOf(point);
                point.GetComponent<TargetPoint>().isLastPoint =
                    point.GetComponent<TargetPoint>().pointIndex == pointsCount - 1;
            }
        }
    }
}