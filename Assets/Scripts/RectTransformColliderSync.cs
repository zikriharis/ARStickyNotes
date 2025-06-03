using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RectTransformColliderSync : MonoBehaviour
{
    void Update()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        BoxCollider boxCollider = GetComponent<BoxCollider>();

        // Get world size by multiplying with lossyScale (handles parent scaling properly)
        Vector2 size = rectTransform.rect.size;
        Vector3 scale = rectTransform.lossyScale;
        float width = size.x * scale.x;
        float height = size.y * scale.y;

        // Set collider size (z-depth is your choice, say 0.01f for a thin plane)
        boxCollider.size = new Vector3(width, height, 0.01f);

        // Optional: set center to zero if pivot is center
        boxCollider.center = Vector3.zero;
    }
}
