using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(BoxCollider))]
public class AutoResizeCollider : MonoBehaviour
{
    [Tooltip("The RectTransform of the Canvas or Panel you want to match.")]
    public RectTransform targetRect;

    [Tooltip("How deep the collider should be (Z axis).")]
    public float depth = 0.01f;

    private BoxCollider boxCollider;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        UpdateColliderSize();
    }

#if UNITY_EDITOR
    void Update()
    {
        // Update live in Editor for visual tuning
        if (!Application.isPlaying)
        {
            UpdateColliderSize();
        }
    }
#endif

    public void UpdateColliderSize()
    {
        if (targetRect == null)
        {
            Debug.LogWarning("Target RectTransform not assigned.");
            return;
        }

        // Convert pixel size to world units (1 unit = 100 pixels)
        Vector2 size = targetRect.sizeDelta;
        Vector3 canvasScale = targetRect.lossyScale;
        float width = (size.x / 100f) * canvasScale.x;
        float height = (size.y / 100f) * canvasScale.y;

        boxCollider.size = new Vector3(width, height, depth);
        boxCollider.center = Vector3.zero;
    }
}
