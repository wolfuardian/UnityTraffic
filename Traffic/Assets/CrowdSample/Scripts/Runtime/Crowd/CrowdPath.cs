using System;
using System.Collections.Generic;
using UnityEngine;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    [ExecuteInEditMode]
    public class CrowdPath : MonoBehaviour
    {
        [NonSerialized] public bool             isInEditMode;
        [NonSerialized] public bool             isOpenPointConfigPanel;
        public                 List<GameObject> waypoints = new List<GameObject>();
        public                 EditMode         editMode;

        public enum EditMode
        {
            None = 0,
            Add  = 1
        }

        public bool isInEditMode1
        {
            get => isInEditMode;
            set => isInEditMode = value;
        }

        private void OnDrawGizmos()
        {
            if (waypoints.Count <= 1) return;

            Gizmos.color = Color.cyan;

            for (var i = 0; i < waypoints.Count - 1; i++)
            {
                if (waypoints[i] != null && waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(waypoints[i].transform.position, waypoints[i + 1].transform.position);
                }
            }

            for (var i = 0; i < waypoints.Count; i++)
            {
                Gizmos.color = i == waypoints.Count - 1 ? Color.green : Color.red;

                if (waypoints[i] == null) continue;
                var lastPointPosition = waypoints[i].transform.position;
                Gizmos.DrawWireSphere(lastPointPosition,
                    waypoints[i].GetComponent<CrowdPathPoint>().allowableRadius * 0.9f);
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

            var meshFilter   = point.AddComponent<MeshFilter>();
            var meshRenderer = point.AddComponent<MeshRenderer>();
            meshFilter.mesh       = CreateSphereMesh();
            meshRenderer.material = new Material(Shader.Find("Standard"));
            point.AddComponent<CrowdPathPoint>();
            waypoints.Add(point);
        }

        private static Mesh CreateSphereMesh()
        {
            return Resources.GetBuiltinResource<Mesh>("New-Sphere.fbx");
        }
    }
}