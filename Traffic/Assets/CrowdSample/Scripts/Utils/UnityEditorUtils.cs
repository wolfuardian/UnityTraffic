using System.Linq;
using UnityEngine;

namespace CrowdSample.Scripts.Utils
{
    public class UnityEditorUtils : UnityEditor.Editor
    {
        public static void UpdateAllGizmos()
        {
            foreach (var updatable in FindObjectsOfType<MonoBehaviour>().OfType<IUpdatableGizmo>())
            {
                updatable.UpdateGizmo();
            }
        }
    }
}