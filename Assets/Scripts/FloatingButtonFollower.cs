using UnityEngine;

public class FloatingButtonFollower : MonoBehaviour
{
    public Transform boardTransform;
    public Vector3 offset = new Vector3(0.5f, 0.55f, 0.5f); // Adjust as needed

    void LateUpdate()
    {
        if (boardTransform == null) return;

        // Match board's position + offset in local space
        transform.position = boardTransform.position + boardTransform.TransformVector(offset);

        transform.localScale = Vector3.one * 1; // like 0.1f or whatever looks right

    }
}
