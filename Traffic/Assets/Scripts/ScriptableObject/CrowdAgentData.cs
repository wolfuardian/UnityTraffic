using UnityEngine;
using System.Collections.Generic;

namespace ScriptableObject
{
    [CreateAssetMenu(fileName = "CrowdAgentData", menuName = "CrowdWizard/New Crowd Agent Data")]
    public class CrowdAgentData : UnityEngine.ScriptableObject
    {
        public List<GameObject> crowdAgentPrefabs = new List<GameObject>();
    }
}