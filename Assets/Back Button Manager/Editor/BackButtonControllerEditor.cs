using UnityEditor;
using UnityEngine;

namespace RHP.BackButtonManager
{
    [CustomEditor(typeof(BackButtonController))]
    public class BackButtonControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);
            if (GUILayout.Button("Back Button Viewer"))
            {
                BackButtonViewer.ShowWindow();
            }      
            
            GUILayout.Space(5);
            if (GUILayout.Button("Action Handler Viewer (UI)"))
            {
                BackButtonActionHandlerScanner.ShowWindow();
            }
        }
    }
}