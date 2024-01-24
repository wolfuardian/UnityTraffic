using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class CrowdWizard : MonoBehaviour
    {
        public GameObject       rootPath;
        public GameObject       rootAgent;
        public List<GameObject> pathInstances  = new List<GameObject>();
        public List<GameObject> agentInstances = new List<GameObject>();

        public bool IsPathCreated  => rootPath != null;
        public bool IsAgentCreated => rootAgent != null;

        public void OnCreatePath()          => CreatePath();
        public void OnCreateAgent()         => CreateAgent();
        public void OnCreatePathInstance()  => CreatePathInstance();
        public void OnCreateAgentInstance() => CreateAgentInstance();

        private void CreatePath()
        {
            if (IsPathCreated) return;

            var newPath = new GameObject("Path");
            newPath.transform.SetParent(transform);

            rootPath = newPath;

            pathInstances.Clear();
        }

        private void CreateAgent()
        {
            if (IsAgentCreated) return;

            var newAgent = new GameObject("Agent");
            newAgent.transform.SetParent(transform);

            rootAgent = newAgent;

            agentInstances.Clear();
        }

        private void CreatePathInstance()
        {
            var newPathInst = new GameObject("Path" + pathInstances.Count);
            newPathInst.transform.SetParent(rootPath.transform);

            var path        = newPathInst.AddComponent<Path>();
            var pathGizmos  = newPathInst.AddComponent<PathGizmos>();
            var pathBuilder = newPathInst.AddComponent<PathBuilder>();

            InternalEditorUtility.SetIsInspectorExpanded(path,        false);
            InternalEditorUtility.SetIsInspectorExpanded(pathGizmos,  true);
            InternalEditorUtility.SetIsInspectorExpanded(pathBuilder, true);

            pathInstances.Add(newPathInst);
        }

        private void CreateAgentInstance()
        {
            var newAgentInst = new GameObject("Agent" + agentInstances.Count);
            newAgentInst.transform.SetParent(rootAgent.transform);

            var agentFactory = newAgentInst.AddComponent<CrowdAgentFactoryController>();

            agentInstances.Add(newAgentInst);
        }
    }
}