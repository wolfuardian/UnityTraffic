using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class CrowdWizard : MonoBehaviour
    {
        #region Field Declarations

        [SerializeField] private List<GameObject> groupInstances = new List<GameObject>();

        #endregion

        #region Properties

        public List<GameObject> GroupInstances => groupInstances;

        #endregion

        #region Public Methods

        public void AddGroupInstance()
        {
            var newGroupInst = new GameObject("CrowdGroup_" + groupInstances.Count);
            newGroupInst.transform.SetParent(transform);
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