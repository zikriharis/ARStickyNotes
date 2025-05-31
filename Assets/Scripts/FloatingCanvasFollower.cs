using UnityEngine;

public class FloatingCanvasFollower : MonoBehaviour
{
    public Transform cubeTransform;     // The cube's transform
    public float yOffset = 0.1f;        // Space above the cube

    void Update()
    {
        float cubeHeight = cubeTransform.localScale.y;
        Vector3 newPosition = cubeTransform.position + new Vector3(0, cubeHeight + yOffset, 0);
        transform.position = newPosition;
    }
}
