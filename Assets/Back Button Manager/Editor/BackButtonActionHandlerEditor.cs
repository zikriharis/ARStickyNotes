using UnityEditor;
using UnityEngine;

namespace RHP.BackButtonManager
{
    [CustomEditor(typeof(BackButtonActionHandler))]
    public class BackButtonActionHandlerEditor : Editor
    {
        private BackButtonActionHandler backButtonActionHandler;

        private SerializedProperty backButtonActionType;
        private SerializedProperty autoRegisterOnStart;
        private SerializedProperty clearAllHandlersOnStart;
        private SerializedProperty triggerTargetHandler;
        private SerializedProperty targetActionHandler;
        private SerializedProperty targetRemoveHandler;
        private SerializedProperty removeHandlerType;

        private void OnEnable()
        {
            backButtonActionHandler = (BackButtonActionHandler)target;

            backButtonActionType = serializedObject.FindProperty(nameof(backButtonActionHandler.backButtonActionType));
            autoRegisterOnStart = serializedObject.FindProperty(nameof(backButtonActionHandler.autoRegisterOnStart));
            clearAllHandlersOnStart = serializedObject.FindProperty(nameof(backButtonActionHandler.clearAllHandlersOnStart));
            triggerTargetHandler = serializedObject.FindProperty(nameof(backButtonActionHandler.triggerTargetHandler));
            targetActionHandler = serializedObject.FindProperty(nameof(backButtonActionHandler.targetActionHandler));
            targetRemoveHandler = serializedObject.FindProperty(nameof(backButtonActionHandler.targetRemoveHandler));
            removeHandlerType = serializedObject.FindProperty(nameof(backButtonActionHandler.removeHandlerType));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(backButtonActionType);

            switch (backButtonActionHandler.backButtonActionType)
            {
                case BackButtonActionTypes.ButtonClick:
                    EditorGUILayout.PropertyField(targetActionHandler, new GUIContent("Target Action Handler"));
                    EditorGUILayout.PropertyField(autoRegisterOnStart);
                    break;
                case BackButtonActionTypes.Trigger:
                    EditorGUILayout.PropertyField(triggerTargetHandler, new GUIContent("Trigger Target Handler"));
                    break;
                case BackButtonActionTypes.Remove:
                    EditorGUILayout.PropertyField(removeHandlerType);
                    if (backButtonActionHandler.removeHandlerType == RemoveHandlerTypes.RemoveTargetHandler)
                    {
                        EditorGUILayout.PropertyField(targetRemoveHandler);
                    }

                    break;
                case BackButtonActionTypes.Quit:
                    EditorGUILayout.PropertyField(clearAllHandlersOnStart);
                    break;
                case BackButtonActionTypes.Wait:
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}