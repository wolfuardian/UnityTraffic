using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace CrowdSample.Scripts.Utils
{
    public class UnityEditorUtils : UnityEditor.Editor
    {
        public static void UpdateAllReceiverImmediately()
        {
            foreach (var updatable in FindObjectsOfType<MonoBehaviour>().OfType<IUpdateReceiver>())
            {
                updatable.UpdateImmediately();
            }
        }

        public static void SetInspectorLock(bool lockState)
        {
            try
            {
                // Get the Inspector window type
                var inspectorWindowType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");
                if (inspectorWindowType == null)
                {
                    throw new InvalidOperationException("Could not find InspectorWindow type.");
                }

                // Get the current Inspector window instance
                var inspectorWindow = EditorWindow.GetWindow(inspectorWindowType);
                if (inspectorWindow == null)
                {
                    throw new InvalidOperationException("Could not find an open Inspector window.");
                }

                // Use reflection to set the lock state
                var isLockedProperty =
                    inspectorWindowType.GetProperty("isLocked", BindingFlags.Public | BindingFlags.Instance);
                if (isLockedProperty == null)
                {
                    throw new InvalidOperationException("Could not find 'isLocked' property in InspectorWindow type.");
                }

                isLockedProperty.SetValue(inspectorWindow, lockState, null);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error setting Inspector lock state: " + ex.Message);
            }
        }

        public static GUIStyle CreateHeaderStyle(FontStyle fontStyle, int fontSize)
        {
            var headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = fontStyle,
                fontSize  = fontSize,
                normal    = { textColor = EditorStyles.boldLabel.normal.textColor }
            };
            return headerStyle;
        }
    }
}