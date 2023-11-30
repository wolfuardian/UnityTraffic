using UnityEditor;
using Runtime.Crowd;

namespace Editor.Crowd
{
    [CustomEditor(typeof(CrowdPathPoint))]
    public class CrowdPathPointEditor : UnityEditor.Editor
    {
        private SerializedProperty _colliderRadiusProp;
        private SerializedProperty _pointIndexProp;
        private SerializedProperty _isLastPointProp;
        private SerializedProperty _allowableRadiusProp;

        private void OnEnable()
        {
            _colliderRadiusProp = serializedObject.FindProperty("colliderRadius");
            _pointIndexProp = serializedObject.FindProperty("pointIndex");
            _isLastPointProp = serializedObject.FindProperty("isLastPoint");
            _allowableRadiusProp = serializedObject.FindProperty("allowableRadius");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_colliderRadiusProp);
            EditorGUILayout.PropertyField(_pointIndexProp);
            EditorGUILayout.PropertyField(_isLastPointProp);
            EditorGUILayout.PropertyField(_allowableRadiusProp);

            serializedObject.ApplyModifiedProperties();
        }
    }
}