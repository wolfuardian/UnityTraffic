using UnityEditor;
using CrowdSample.Scripts.Utils;
using CrowdSample.Scripts.Runtime.Data;
using UnityEngine;

namespace CrowdSample.Scripts.Editor.Data
{
    [CustomEditor(typeof(AgentGenerationConfig))]
    public class AgentGenerationConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var config = (AgentGenerationConfig)target;

            config.generationMode =
                (AgentGenerationConfig.GenerationMode)EditorGUILayout.EnumPopup("生成模式", config.generationMode);

            switch (config.generationMode)
            {
                case AgentGenerationConfig.GenerationMode.InfinityFlow:
                    DisplayInfinityFlowOptions(config);
                    break;
                case AgentGenerationConfig.GenerationMode.MultipleCircle:
                    DisplayMultipleCircleOptions(config);
                    break;
                case AgentGenerationConfig.GenerationMode.SingleCircle:
                    DisplaySingleCircleOptions(config);
                    break;
                case AgentGenerationConfig.GenerationMode.Custom:
                    DrawCustomOptions(config);
                    break;
            }

            config.ApplyPresetProperties();

            UnityEditorUtils.UpdateAllGizmos();
            SceneView.RepaintAll();

            serializedObject.ApplyModifiedProperties();
        }

        private static void DisplayInfinityFlowOptions(AgentGenerationConfig config)
        {
            EditorGUILayout.HelpBox("InfinityFlow：按指定的速率持續生成代理。", MessageType.Info);
            DrawPerSecondCountOption(config);
            DrawMaxCountOption(config);
        }

        private static void DisplayMultipleCircleOptions(AgentGenerationConfig config)
        {
            EditorGUILayout.HelpBox("MultipleCircle：一次性生成指定數量的代理。", MessageType.Info);
            DrawInstantCountOption(config);
            DrawMaxCountOption(config);
            DrawOffsetOption(config);
            DrawUseSpacingOption(config);
            if (config.IsUseSpacing)
            {
                DrawSpacingOption(config);
            }
        }

        private static void DisplaySingleCircleOptions(AgentGenerationConfig config)
        {
            EditorGUILayout.HelpBox("SingleCircle：生成單個代理，並可設置路徑為開放或封閉。", MessageType.Info);
            DrawClosedPathOption(config);
            DrawOffsetOption(config);
        }

        private static void DrawCustomOptions(AgentGenerationConfig config)
        {
            EditorGUILayout.HelpBox("Custom：允許自定義所有參數。", MessageType.Info);
            DrawSpawnAgentOnceOption(config);
            DrawClosedPathOption(config);
            DrawInstantCountOption(config);
            DrawPerSecondCountOption(config);
            DrawMaxCountOption(config);
            DrawOffsetOption(config);
            DrawUseSpacingOption(config);
            if (config.IsUseSpacing)
            {
                DrawSpacingOption(config);
            }
        }

        private static void DrawSpawnAgentOnceOption(AgentGenerationConfig config)
        {
            config.IsSpawnAgentOnce = EditorGUILayout.Toggle("一次性生成代理", config.IsSpawnAgentOnce);
        }

        private static void DrawClosedPathOption(AgentGenerationConfig config)
        {
            config.IsClosedPath = EditorGUILayout.Toggle("封閉路徑", config.IsClosedPath);
        }

        private static void DrawInstantCountOption(AgentGenerationConfig config)
        {
            config.InstantCount = EditorGUILayout.IntSlider("一次性生成數量", config.InstantCount, 1, 100);
        }

        private static void DrawPerSecondCountOption(AgentGenerationConfig config)
        {
            config.PerSecondCount = EditorGUILayout.IntSlider("生成數量 / 秒", config.PerSecondCount, 1, 10);
        }

        private static void DrawMaxCountOption(AgentGenerationConfig config)
        {
            config.MaxCount = EditorGUILayout.IntSlider("最大生成數量", config.MaxCount, 1, 100);
        }

        private static void DrawOffsetOption(AgentGenerationConfig config)
        {
            config.Offset = EditorGUILayout.FloatField("偏移", config.Offset);
            config.Offset = Mathf.Clamp(config.Offset, 0, float.MaxValue);
        }

        private static void DrawUseSpacingOption(AgentGenerationConfig config)
        {
            config.IsUseSpacing = EditorGUILayout.Toggle("使用間距", config.IsUseSpacing);
        }

        private static void DrawSpacingOption(AgentGenerationConfig config)
        {
            config.Spacing = EditorGUILayout.Slider("間距", config.Spacing, 1f, 10f);
        }
    }
}