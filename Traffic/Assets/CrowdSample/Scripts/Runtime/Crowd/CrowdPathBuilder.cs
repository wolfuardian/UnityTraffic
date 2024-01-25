using UnityEngine;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    [RequireComponent(typeof(CrowdPath))]
    [RequireComponent(typeof(CrowdPathGizmos))]
    [ExecuteInEditMode]
    public class CrowdPathBuilder : MonoBehaviour
    {
        #region Properties

        [SerializeField] private CrowdPath crowdPath;

        #endregion

        #region Properties

        public CrowdPath CrowdPath => crowdPath ??= GetComponent<CrowdPath>();

        #endregion

        public bool     isOpenPointConfigPanel;
        public EditMode editMode;

        public enum EditMode
        {
            None = 0,
            Add  = 1
        }


#if UNITY_EDITOR
        private void Awake()
        {
            if (crowdPath == null) crowdPath = GetComponent<CrowdPath>();
        }
#endif
    }
}