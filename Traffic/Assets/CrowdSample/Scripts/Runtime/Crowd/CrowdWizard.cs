using UnityEngine;
using UnityEditorInternal;
using System.Collections.Generic;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class CrowdWizard : MonoBehaviour
    {
        #region Field Declarations

        [SerializeField] private List<GameObject> generatorInstances = new List<GameObject>();

        #endregion

        #region Properties

        public List<GameObject> GeneratorInstances => generatorInstances;

        #endregion

        #region Public Methods

        public void AddGroupInstance()
        {
            var newGeneratorInst = new GameObject("CrowdGenerator_" + generatorInstances.Count);
            newGeneratorInst.transform.SetParent(transform);
            generatorInstances.Add(newGeneratorInst);
            ConfigureGeneratorInstance(newGeneratorInst);
        }

        #endregion

        #region Private Methods

        private static void ConfigureGeneratorInstance(GameObject inst)
        {
            var crowdGenerator = inst.AddComponent<CrowdGenerator>();
            InternalEditorUtility.SetIsInspectorExpanded(crowdGenerator, true);
        }

        #endregion
    }
}