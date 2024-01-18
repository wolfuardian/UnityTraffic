using UnityEditor;
using CrowdSample.Scripts.Runtime.Data;

namespace CrowdSample.Scripts.Editor.Data
{
    [CustomEditor(typeof(AgentGenerationConfig))]
    public class AgentSpawnerConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var config = (AgentGenerationConfig)target;

            config.generationMode =
                (AgentGenerationConfig.GenerationMode)EditorGUILayout.EnumPopup("生成模式", config.generationMode);

            switch (config.generationMode)
            {
                case AgentGenerationConfig.GenerationMode.InfinityFlow:
                    EditorGUILayout.HelpBox("InfinityFlow：按指定的速率持續生成代理。",
                        MessageType.Info);
                    config.PerSecondCount =
                        EditorGUILayout.IntSlider("每秒生成數量", config.PerSecondCount, 1, 10);
                    config.MaxCount =
                        EditorGUILayout.IntSlider("最大生成數量", config.MaxCount, 1, 100);
                    break;
                case AgentGenerationConfig.GenerationMode.MultipleCircle:
                    EditorGUILayout.HelpBox("MultipleCircle：一次性生成指定數量的代理。",
                        MessageType.Info);
                    config.InstantCount =
                        EditorGUILayout.IntSlider("一次性生成數量", config.InstantCount, 1, 100);
                    break;
                case AgentGenerationConfig.GenerationMode.SingleCircle:
                    EditorGUILayout.HelpBox("SingleCircle：生成單個代理，並可設置路徑為開放或封閉。",
                        MessageType.Info);
                    config.ClosedPath = EditorGUILayout.Toggle("封閉路徑", config.ClosedPath);
                    break;
                case AgentGenerationConfig.GenerationMode.Custom:
                    EditorGUILayout.HelpBox("Custom：允許自定義所有參數。",
                        MessageType.Info);
                    config.SpawnAgentOnce = EditorGUILayout.Toggle("一次性生成代理", config.SpawnAgentOnce);
                    config.ClosedPath     = EditorGUILayout.Toggle("封閉路徑",    config.ClosedPath);
                    config.InstantCount =
                        EditorGUILayout.IntSlider("一次性生成數量", config.InstantCount, 1, 100);
                    config.PerSecondCount =
                        EditorGUILayout.IntSlider("每秒生成數量", config.PerSecondCount, 1, 10);
                    config.MaxCount =
                        EditorGUILayout.IntSlider("最大生成數量", config.MaxCount, 1, 100);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}