using UnityEngine;

public class StickyNoteTransformAdjuster : MonoBehaviour
{
    [Header("Set the desired local scale for spawned sticky notes")]
    public Vector3 desiredLocalScale = Vector3.one;

    [Header("Set the desired local rotation (Euler angles) for spawned sticky notes")]
    public Vector3 desiredLocalEulerAngles = Vector3.zero;

    void Awake()
    {
        // Apply the desired local scale
        transform.localScale = desiredLocalScale;

        // Apply the desired local rotation
        transform.localEulerAngles = desiredLocalEulerAngles;
    }
}