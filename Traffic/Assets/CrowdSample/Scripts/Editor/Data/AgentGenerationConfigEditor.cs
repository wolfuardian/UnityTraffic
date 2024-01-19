using UnityEditor;
using CrowdSample.Scripts.Utils;
using CrowdSample.Scripts.Runtime.Data;

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
                    DisplayCustomOptions(config);
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
            config.PerSecondCount = EditorGUILayout.IntSlider("生成數量 / 秒", config.PerSecondCount, 1, 10);
            config.MaxCount       = EditorGUILayout.IntSlider("最大生成數量", config.MaxCount,       1, 100);
        }

        private static void DisplayMultipleCircleOptions(AgentGenerationConfig config)
        {
            EditorGUILayout.HelpBox("MultipleCircle：一次性生成指定數量的代理。", MessageType.Info);
            config.InstantCount = EditorGUILayout.IntSlider("一次性生成數量", config.InstantCount, 1, 100);
            config.MaxCount     = EditorGUILayout.IntSlider("最大生成數量",  config.MaxCount,     1, 100);
            config.Offset       = EditorGUILayout.Slider("偏移", config.Offset, 0f, 100f);
            config.UseSpacing   = EditorGUILayout.Toggle("使用間距", config.UseSpacing);
            if (config.UseSpacing)
            {
                config.Spacing = EditorGUILayout.Slider("間距", config.Spacing, 1f, 10f);
            }
        }

        private static void DisplaySingleCircleOptions(AgentGenerationConfig config)
        {
            EditorGUILayout.HelpBox("SingleCircle：生成單個代理，並可設置路徑為開放或封閉。", MessageType.Info);
            config.ClosedPath = EditorGUILayout.Toggle("封閉路徑", config.ClosedPath);
        }

        private static void DisplayCustomOptions(AgentGenerationConfig config)
        {
            EditorGUILayout.HelpBox("Custom：允許自定義所有參數。", MessageType.Info);
            config.SpawnAgentOnce = EditorGUILayout.Toggle("一次性生成代理", config.SpawnAgentOnce);
            config.ClosedPath     = EditorGUILayout.Toggle("封閉路徑",    config.ClosedPath);
            config.InstantCount   = EditorGUILayout.IntSlider("一次性生成數量", config.InstantCount,   1, 100);
            config.PerSecondCount = EditorGUILayout.IntSlider("生成數量 / 秒",  config.PerSecondCount, 1, 10);
            config.MaxCount       = EditorGUILayout.IntSlider("最大生成數量",  config.MaxCount,       1, 100);
            config.Offset         = EditorGUILayout.Slider("偏移", config.Offset, 0f, 100f);
            config.UseSpacing     = EditorGUILayout.Toggle("使用間距", config.UseSpacing);
            if (config.UseSpacing)
            {
                config.Spacing = EditorGUILayout.Slider("間距", config.Spacing, 1f, 10f);
            }
        }
    }
}