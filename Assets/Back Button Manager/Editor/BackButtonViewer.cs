using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace RHP.BackButtonManager
{
    public class BackButtonViewer : EditorWindow
    {
        private double escMessageStartTime = -1;
        private const double escMessageDuration = 1;

        [MenuItem("Tools/RHP/Back Button Manager/Back Button Viewer")]
        public static void ShowWindow()
        {
            GetWindow<BackButtonViewer>("Back Button Viewer");
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            SubscribeToStackEvent();
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            UnsubscribeFromStackEvent();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                SubscribeToStackEvent();
            }
            else if (state == PlayModeStateChange.ExitingPlayMode)
            {
                UnsubscribeFromStackEvent();
            }
        }

        private void SubscribeToStackEvent()
        {
            if (EditorApplication.isPlaying && BackButtonController.Instance != null)
            {
                BackButtonController.Instance.OnStatusChanged += OnStatusChanged;
                BackButtonController.Instance.OnEscapePressed += OnEscapePressed;
            }
        }

        private void UnsubscribeFromStackEvent()
        {
            if (EditorApplication.isPlaying && BackButtonController.Instance != null)
            {
                BackButtonController.Instance.OnStatusChanged -= OnStatusChanged;
                BackButtonController.Instance.OnEscapePressed -= OnEscapePressed;
            }
        }

        private void OnStatusChanged()
        {
            Repaint();
        }

        private void OnEscapePressed()
        {
            escMessageStartTime = EditorApplication.timeSinceStartup;
            Repaint();
        }

        private void OnGUI()
        {
            HandleEscKey();

            if (!EditorApplication.isPlaying)
            {
                EditorGUILayout.LabelField("Enter Play Mode to view Back Button details.");
                return;
            }

            if (BackButtonController.Instance == null)
            {
                EditorGUILayout.LabelField("BackButtonController instance not found in the scene!");
                return;
            }

            var stackArray = BackButtonController.Instance.GetRegisteredHandlers();

            string stackInfo = "Back Button Action Count: " + stackArray.Length;
            if (stackArray.Length == 0)
            {
                stackInfo += " (Exit on Back: " + (BackButtonController.Instance.IsQuit ? "Enabled" : "Disabled") + ")";
            }

            EditorGUILayout.LabelField(stackInfo);

            foreach (var controller in stackArray.Reverse())
            {
                if (controller != null)
                {
                    string hierarchyPath = GetGameObjectHierarchy(controller.gameObject);
                    EditorGUILayout.LabelField("Action: " + hierarchyPath);
                }
            }

            if (BackButtonController.Instance.IsWait)
            {
                EditorGUILayout.LabelField("Waiting..");
            }
        }

        private void HandleEscKey()
        {
            double elapsed = EditorApplication.timeSinceStartup - escMessageStartTime;
            if (elapsed < escMessageDuration)
            {
                float alpha = 1f - (float)(elapsed / escMessageDuration);

                GUIStyle escStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 20,
                    alignment = TextAnchor.MiddleCenter
                };
                escStyle.normal.textColor = new Color(1f, 1f, 1f, alpha);

                Rect rect = new Rect(0, position.height - 50, position.width, 50);
                EditorGUI.DrawRect(rect, new Color(0.2f, 0.6f, 1f, alpha));
                GUI.Label(rect, "Esc Pressed", escStyle);

                Repaint();
            }
        }

        private string GetGameObjectHierarchy(GameObject obj)
        {
            List<string> names = new List<string>();
            Transform current = obj.transform;
            while (current != null)
            {
                names.Add(current.name);
                current = current.parent;
            }

            names.Reverse();
            return string.Join(" -> ", names);
        }
    }
}