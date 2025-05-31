using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RectTransformColliderSync : MonoBehaviour
{
    void Update()
    {
        var rectTransform = GetComponent<RectTransform>();
        var boxCollider = GetComponent<BoxCollider>();
        // Convert RectTransform size to world space size
        Vector3 size = rectTransform.TransformVector(rectTransform.rect.size);
        // Set collider size (x = width, y = height, z = depth)
        boxCollider.size = new Vector3(size.x, size.y, boxCollider.size.z);
        // Reset center if needed
        boxCollider.center = Vector3.zero;
    }
}