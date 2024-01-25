using UnityEditor;
using UnityEngine;
using CrowdSample.Scripts.Utils;
using CrowdSample.Scripts.Runtime.Data;

namespace CrowdSample.Scripts.Editor.Data
{
    [CustomEditor(typeof(CrowdGenerationConfig))]
    public class CrowdGenerationConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var config = (CrowdGenerationConfig)target;

            config.generationMode =
                (CrowdGenerationConfig.GenerationMode)EditorGUILayout.EnumPopup("生成模式", config.generationMode);

            switch (config.generationMode)
            {
                case CrowdGenerationConfig.GenerationMode.InfinityFlow:
                    DisplayInfinityFlowOptions(config);
                    break;
                case CrowdGenerationConfig.GenerationMode.MultipleCircle:
                    DisplayMultipleCircleOptions(config);
                    break;
                case CrowdGenerationConfig.GenerationMode.SingleCircle:
                    DisplaySingleCircleOptions(config);
                    break;
                case CrowdGenerationConfig.GenerationMode.Custom:
                    DrawCustomOptions(config);
                    break;
            }

            config.ApplyPresetProperties();

            if (GUI.changed)
            {
                UnityEditorUtils.UpdateAllImmediately();
            }

            SceneView.RepaintAll();

            serializedObject.ApplyModifiedProperties();
        }

        private static void DisplayInfinityFlowOptions(CrowdGenerationConfig config)
        {
            EditorGUILayout.HelpBox("InfinityFlow：按指定的生成間隔持續生成代理。", MessageType.Info);
            DrawSpawnIntervalOption(config);
            DrawMaxCountOption(config);
        }

        private static void DisplayMultipleCircleOptions(CrowdGenerationConfig config)
        {
            EditorGUILayout.HelpBox("MultipleCircle：一次性生成指定數量的代理。", MessageType.Info);
            DrawInstantCountOption(config);
            DrawReverseDirectionOption(config);
            DrawMaxCountOption(config);
            DrawOffsetOption(config);
            DrawUseSpacingOption(config);
            if (config.IsUseSpacing)
            {
                DrawSpacingOption(config);
            }
        }

        private static void DisplaySingleCircleOptions(CrowdGenerationConfig config)
        {
            EditorGUILayout.HelpBox("SingleCircle：生成單個代理，並可設置路徑為開放或封閉。", MessageType.Info);
            DrawClosedPathOption(config);
            DrawReverseDirectionOption(config);
            DrawOffsetOption(config);
        }

        private static void DrawCustomOptions(CrowdGenerationConfig config)
        {
            EditorGUILayout.HelpBox("Custom：允許自定義所有參數。", MessageType.Info);
            DrawSpawnAgentOnceOption(config);
            DrawClosedPathOption(config);
            DrawReverseDirectionOption(config);
            DrawInstantCountOption(config);
            DrawSpawnIntervalOption(config);
            DrawMaxCountOption(config);
            DrawOffsetOption(config);
            DrawUseSpacingOption(config);
            if (config.IsUseSpacing)
            {
                DrawSpacingOption(config);
            }
        }

        private static void DrawSpawnAgentOnceOption(CrowdGenerationConfig config)
        {
            config.IsSpawnAgentOnce = EditorGUILayout.Toggle("一次性生成代理", config.IsSpawnAgentOnce);
        }

        private static void DrawClosedPathOption(CrowdGenerationConfig config)
        {
            config.IsClosedPath = EditorGUILayout.Toggle("封閉路徑", config.IsClosedPath);
        }

        private static void DrawReverseDirectionOption(CrowdGenerationConfig config)
        {
            config.IsReverseDirection = EditorGUILayout.Toggle("反轉路徑", config.IsReverseDirection);
        }

        private static void DrawInstantCountOption(CrowdGenerationConfig config)
        {
            config.InstantCount = EditorGUILayout.IntSlider("一次性生成數量", config.InstantCount, 1, 100);
        }

        private static void DrawSpawnIntervalOption(CrowdGenerationConfig config)
        {
            config.SpawnInterval = EditorGUILayout.Slider("生成間隔", config.SpawnInterval, 0.1f, 10f);
        }

        private static void DrawMaxCountOption(CrowdGenerationConfig config)
        {
            config.MaxCount = EditorGUILayout.IntSlider("最大生成數量", config.MaxCount, 1, 100);
        }

        private static void DrawOffsetOption(CrowdGenerationConfig config)
        {
            config.Offset = EditorGUILayout.FloatField("偏移", config.Offset);
            config.Offset = Mathf.Clamp(config.Offset, 0, float.MaxValue);
        }

        private static void DrawUseSpacingOption(CrowdGenerationConfig config)
        {
            config.IsUseSpacing = EditorGUILayout.Toggle("使用間距", config.IsUseSpacing);
        }

        private static void DrawSpacingOption(CrowdGenerationConfig config)
        {
            config.Spacing = EditorGUILayout.Slider("間距", config.Spacing, 1f, 10f);
        }
    }
}