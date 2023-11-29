// using Runtime.Crowd;
// using UnityEditor;
// using UnityEngine;
//
// namespace Editor.Crowd
// {
//     [CustomEditor(typeof(CrowdPathGuide))]
//     public class CrowdPathGuideEditor : UnityEditor.Editor
//     {
//         private CrowdPathGuide _crowdPathGuide;
//         private SerializedProperty _waypointsProp;
//         private SerializedProperty _lineRendererProp;
//         private SerializedProperty _showGuideInEditorProp;
//
//
//         private void OnEnable()
//         {
//             _crowdPathGuide = (CrowdPathGuide)target;
//             _waypointsProp = serializedObject.FindProperty("target");
//             _lineRendererProp = serializedObject.FindProperty("lineRenderer");
//             _showGuideInEditorProp = serializedObject.FindProperty("showGuideInEditor");
//         }
//
//         public override void OnInspectorGUI()
//         {
//             EditorGUILayout.PropertyField(_waypointsProp);
//             EditorGUILayout.PropertyField(_lineRendererProp);
//             EditorGUILayout.PropertyField(_showGuideInEditorProp);
//
//             if (_crowdPathGuide.GetComponent<LineRenderer>() == null)
//             {
//                 _crowdPathGuide.gameObject.AddComponent<LineRenderer>();
//                 _crowdPathGuide.lineRenderer = _crowdPathGuide.GetComponent<LineRenderer>();
//                 _crowdPathGuide.InitialGuideline();
//             }
//
//             if (_crowdPathGuide.lineRenderer != null)
//             {
//                 if (_crowdPathGuide.showGuideInEditor)
//                 {
//                     _crowdPathGuide.lineRenderer.enabled = true;
//                     _crowdPathGuide.RedrawGuide();
//                 }
//                 else
//                 {
//                     _crowdPathGuide.lineRenderer.enabled = false;
//                 }
//             }
//
//
//             serializedObject.ApplyModifiedProperties();
//         }
//     }
// }