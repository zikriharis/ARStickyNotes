using UnityEngine;

public class CanvasCameraSetter : MonoBehaviour
{
    void Awake()
    {
        var canvas = GetComponent<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
        {
            // Replace "Camera.main" with a reference to your ARCamera if needed
            canvas.worldCamera = Camera.main;
        }
    }
}