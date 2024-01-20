using UnityEngine;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    [ExecuteInEditMode]
    public class PathBuilder : MonoBehaviour
    {
        [SerializeField] private Path path;

        public Path Path => path ??= GetComponent<Path>();

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
            if (path == null) path = GetComponent<Path>();
        }
#endif
    }
}