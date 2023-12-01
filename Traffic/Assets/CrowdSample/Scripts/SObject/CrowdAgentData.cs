using System.Collections.Generic;
using UnityEngine;

namespace CrowdSample.Scripts.SObject
{
    [CreateAssetMenu(fileName = "CrowdAgentData", menuName = "CrowdWizard/New Crowd Agent Data")]
    public class CrowdAgentData : ScriptableObject
    {
        public List<GameObject> crowdAgentPrefabs = new List<GameObject>();

        public int maxAgentCount = 5;
        public float spawnInterval = 1f;

        public float minSpeed = 5f;
        public float maxSpeed = 10f;

        public float turningRadius = 2f;
        public float stoppingDistance = 1f;
    }
}