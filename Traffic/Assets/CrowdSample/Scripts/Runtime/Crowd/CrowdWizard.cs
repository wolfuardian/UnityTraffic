using System.Collections.Generic;
using UnityEngine;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class CrowdWizard : MonoBehaviour
    {
        public GameObject path;
        public GameObject crowdAgent;
        public List<GameObject> pathInstances = new List<GameObject>();
        public List<GameObject> crowdAgentInstances = new List<GameObject>();


        public bool IsPathCreated()
        {
            return path != null;
        }

        public void CreatePath()
        {
            if (IsPathCreated()) return;

            path = new GameObject("Path");
            path.transform.SetParent(transform);
        }

        public void CreatePathInstance()
        {
            var pathInstance = new GameObject("Path" + pathInstances.Count);
            pathInstance.transform.SetParent(path.transform);

            pathInstance.AddComponent<CrowdPath>();

            pathInstances.Add(pathInstance);
        }

        public bool IsCrowdAgentCreated()
        {
            return crowdAgent != null;
        }

        public void CreateCrowdAgent()
        {
            if (IsCrowdAgentCreated()) return;

            crowdAgent = new GameObject("Agent");
            crowdAgent.transform.SetParent(transform);
        }

        public void CreateCrowdAgentInstance()
        {
            var crowdAgentInstance = new GameObject("Agent" + crowdAgentInstances.Count);
            crowdAgentInstance.transform.SetParent(crowdAgent.transform);

            crowdAgentInstance.AddComponent<CrowdAgentFactory>();

            crowdAgentInstances.Add(crowdAgentInstance);
        }
    }
}