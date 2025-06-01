using UnityEngine;

/// <summary>
/// Attach to your Canvas (not as a child of the board).
/// Keeps the Canvas aligned with the board, with an offset along the board's normal.
/// </summary>
[ExecuteAlways]
public class StickyNoteCanvasFollower : MonoBehaviour
{
    public Transform boardTransform; // Assign the board
    public RectTransform canvasRect;
    public float boardBaseWidth = 1f;
    public float boardBaseHeight = 1f;

    [Tooltip("Offset along the board's normal (local up). Positive values move canvas above the board.")]
    public float normalOffset = 0.01f;

    void LateUpdate()
    {
        if (boardTransform == null || canvasRect == null)
            return;

        // 1. Position: Match board's position, add offset along board's local up
        Vector3 boardNormal = boardTransform.up; // Local up is normal to board
        transform.position = boardTransform.position + boardNormal * normalOffset;

        // 2. Rotation: Match board's rotation plus X rotation offset (see previous messages if needed)
        transform.rotation = boardTransform.rotation * Quaternion.Euler(90, 0, 0);

        // 3. Scale: Always (1,1,1)
        transform.localScale = Vector3.one;

        // 4. Size: Match board's scaled width/height
        float width = boardBaseWidth * boardTransform.localScale.x;
        float height = boardBaseHeight * boardTransform.localScale.z;
        canvasRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        canvasRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }
}