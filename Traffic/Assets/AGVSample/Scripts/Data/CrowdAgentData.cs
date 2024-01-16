using System.Collections.Generic;
using UnityEngine;

namespace CrowdSample.Scripts.Data
{
    [CreateAssetMenu(fileName = "CrowdAgentData", menuName = "CrowdWizard/New Crowd Agent Data")]
    public class CrowdAgentData : ScriptableObject
    {
        public List<GameObject> crowdAgentPrefabs = new List<GameObject>();

        public string permissionID = string.Empty;

        public int   maxAgentCount = 5;
        public float spawnInterval = 1f;

        public float minSpeed = 4f;
        public float maxSpeed = 6f;

        public float angularSpeed = 100f;
        public float acceleration = 5f;

        public float turningRadius    = 2f;
        public float stoppingDistance = 1f;
    }
}