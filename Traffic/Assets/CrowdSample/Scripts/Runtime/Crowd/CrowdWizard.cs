using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class CrowdWizard : MonoBehaviour
    {
        #region Field Declarations

        [SerializeField] internal GameObject       groupRoot;
        [SerializeField] internal List<GameObject> groupInstances = new List<GameObject>();

        #endregion

        #region Properties

        public bool Initialized => groupRoot != null;

        #endregion

        #region Public Methods

        public void CreateGroupRoot()
        {
            if (Initialized) return;

            var newGroupRoot = new GameObject("GroupRoot");
            newGroupRoot.transform.SetParent(transform);

            groupRoot = newGroupRoot;

            groupInstances.Clear();
        }

        public void AddGroupInstance()
        {
            var newGroupInst = new GameObject("CrowdGroup_" + groupInstances.Count);
            newGroupInst.transform.SetParent(groupRoot.transform);
            groupInstances.Add(newGroupInst);
            ConfigureGroupInstance(newGroupInst);
        }

        #endregion

        #region Private Methods

        private static void ConfigureGroupInstance(GameObject inst)
        {
            // var path        = inst.AddComponent<Path>();
            // var pathGizmos  = inst.AddComponent<PathGizmos>();
            // var pathBuilder = inst.AddComponent<PathBuilder>();

            // InternalEditorUtility.SetIsInspectorExpanded(path,        false);
            // InternalEditorUtility.SetIsInspectorExpanded(pathGizmos,  true);
            // InternalEditorUtility.SetIsInspectorExpanded(pathBuilder, true);
        }

        #endregion
    }
}