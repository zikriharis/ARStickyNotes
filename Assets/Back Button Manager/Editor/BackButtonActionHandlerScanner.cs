using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RHP.BackButtonManager
{
    public class BackButtonActionHandlerScanner : EditorWindow
    {
        private Vector2 commonScrollPos;
        private List<ScanResult> scanResults = new();

        [MenuItem("Tools/RHP/Back Button Manager/Action Handler Scanner (UI)")]
        public static void ShowWindow()
        {
            GetWindow<BackButtonActionHandlerScanner>("Back Button UI Scanner");
        }

        private void OnEnable()
        {
            ScanScenes();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Build Included Scene Scan", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (GUILayout.Button("Scan Scenes"))
            {
                ScanScenes();
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.Width(position.width / 2));
            EditorGUILayout.LabelField("Scanned Handlers", EditorStyles.boldLabel);
            commonScrollPos = EditorGUILayout.BeginScrollView(commonScrollPos);
            foreach (var result in scanResults)
            {
                EditorGUILayout.LabelField("Scene: " + result.SceneName, EditorStyles.boldLabel);
                foreach (var handlerInfo in result.HandlerInfos)
                {
                    EditorGUILayout.LabelField("  > " + handlerInfo.HierarchyPath);
                }

                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUILayout.Width(position.width / 2));
            EditorGUILayout.LabelField("Handler Properties", EditorStyles.boldLabel);
            commonScrollPos = EditorGUILayout.BeginScrollView(commonScrollPos);
            foreach (var result in scanResults)
            {
                EditorGUILayout.LabelField("Scene: " + result.SceneName, EditorStyles.boldLabel);
                foreach (var handlerInfo in result.HandlerInfos)
                {
                    string detailsLine = string.Join("    ", handlerInfo.Details);
                    EditorGUILayout.LabelField(detailsLine);
                }

                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void ScanScenes()
        {
            scanResults.Clear();

            Scene currentActiveScene = EditorSceneManager.GetActiveScene();

            foreach (EditorBuildSettingsScene buildScene in EditorBuildSettings.scenes)
            {
                if (!buildScene.enabled)
                    continue;

                string scenePath = buildScene.path;
                Scene sceneToScan;
                bool isCurrentActive = false;

                if (scenePath == currentActiveScene.path)
                {
                    sceneToScan = currentActiveScene;
                    isCurrentActive = true;
                }
                else
                {
                    sceneToScan = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                }

                ScanResult result = new ScanResult();
                result.SceneName = sceneToScan.name;
                result.HandlerInfos = new List<HandlerInfo>();

                GameObject[] rootObjects = sceneToScan.GetRootGameObjects();
                List<BackButtonActionHandler> handlersInScene = new List<BackButtonActionHandler>();
                foreach (var root in rootObjects)
                {
                    handlersInScene.AddRange(root.GetComponentsInChildren<BackButtonActionHandler>(true));
                }

                foreach (BackButtonActionHandler handler in handlersInScene)
                {
                    HandlerInfo info = new HandlerInfo();
                    info.HierarchyPath = GetHierarchyPath(handler.gameObject.transform);
                    info.Details = new List<string>();

                    info.Details.Add("> Action Type: " + handler.backButtonActionType.ToString());

                    switch (handler.backButtonActionType)
                    {
                        case BackButtonActionTypes.ButtonClick:
                        {
                            string targetActionHandlerName = (handler.targetActionHandler != null) ? handler.targetActionHandler.gameObject.name : "null";
                            info.Details.Add("> Target Action Handler: " + targetActionHandlerName);
                            info.Details.Add("> autoRegisterOnStart: " + handler.autoRegisterOnStart.ToString());
                            break;
                        }
                        case BackButtonActionTypes.Trigger:
                        {
                            string triggerTargetHandlerName = (handler.triggerTargetHandler != null) ? handler.triggerTargetHandler.gameObject.name : "null";
                            info.Details.Add("> Trigger Target Handler: " + triggerTargetHandlerName);
                            break;
                        }
                        case BackButtonActionTypes.Remove:
                        {
                            info.Details.Add("> Remove Handler Type: " + handler.removeHandlerType.ToString());
                            if (handler.removeHandlerType == RemoveHandlerTypes.RemoveTargetHandler)
                            {
                                string targetRemoveHandlerName = (handler.targetRemoveHandler != null) ? handler.targetRemoveHandler.gameObject.name : "null";
                                info.Details.Add("> Target Remove Handler: " + targetRemoveHandlerName);
                            }

                            break;
                        }
                        case BackButtonActionTypes.Quit:
                        {
                            info.Details.Add("> clearAllHandlersOnStart: " + handler.clearAllHandlersOnStart.ToString());
                            break;
                        }
                        case BackButtonActionTypes.Wait:
                        {
                            break;
                        }
                    }

                    result.HandlerInfos.Add(info);
                }

                scanResults.Add(result);

                if (!isCurrentActive)
                {
                    EditorSceneManager.CloseScene(sceneToScan, true);
                }
            }

            EditorSceneManager.SetActiveScene(currentActiveScene);
        }

        private string GetHierarchyPath(Transform transform)
        {
            if (transform.parent == null)
                return transform.name;
            return GetHierarchyPath(transform.parent) + " > " + transform.name;
        }

        private class ScanResult
        {
            public string SceneName;
            public List<HandlerInfo> HandlerInfos;
        }

        private class HandlerInfo
        {
            public string HierarchyPath;
            public List<string> Details;
        }
    }
}