using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public Camera arCamera;

    void Update()
    {
        if (arCamera != null)
        {
            transform.LookAt(transform.position + arCamera.transform.rotation * Vector3.forward,
                             arCamera.transform.rotation * Vector3.up);
        }
    }
}
