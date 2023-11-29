using System.Collections.Generic;
using UnityEngine;

namespace SObject
{
    [CreateAssetMenu(fileName = "CrowdAgentData", menuName = "CrowdWizard/New Crowd Agent Data")]
    public class CrowdAgentData : ScriptableObject
    {
        public List<GameObject> crowdAgentPrefabs = new List<GameObject>();
    }
}