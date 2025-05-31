using UnityEngine;

public class StickyNoteSpawner : MonoBehaviour
{
    [Header("Prefab & Board")]
    public GameObject stickyNotePrefab;
    public Transform boardTransform;

    [Header("Sticky Note Transform Settings")]
    [Tooltip("Y offset from the board to render the sticky note on top of the board (in world units).")]
    public float yOffset = 0.01f;

    [Tooltip("Local X offset from board center (left/right, in world units).")]
    public float xOffset = 0f;

    [Tooltip("Local Z offset from board center (up/down, in world units).")]
    public float zOffset = 0f;

    [Tooltip("Uniform scale for the sticky note (maintains aspect ratio).")]
    public float stickyNoteScale = 1f;

    [Tooltip("Euler angles (degrees) for sticky note rotation relative to board.")]
    public Vector3 stickyNoteRotation = Vector3.zero;

    public void SpawnStickyNote()
    {
        if (stickyNotePrefab == null || boardTransform == null)
        {
            Debug.LogWarning("StickyNoteSpawner: Assign the stickyNotePrefab and boardTransform in the inspector.");
            return;
        }

        // Calculate spawn position relative to the board
        // X is left/right, Y is depth (distance from camera), Z is up/down
        Vector3 spawnPosition = boardTransform.position
            + boardTransform.right * xOffset    // left/right
            + boardTransform.up * zOffset       // up/down
            + boardTransform.forward * yOffset; // depth (in front, on top of board)

        // Calculate rotation: start with board's rotation, then add user adjustment
        Quaternion spawnRotation = boardTransform.rotation * Quaternion.Euler(stickyNoteRotation);

        // Instantiate and set transform
        GameObject stickyNote = Instantiate(stickyNotePrefab, spawnPosition, spawnRotation, boardTransform);

        // Apply uniform scale for aspect ratio
        stickyNote.transform.localScale = Vector3.one * stickyNoteScale;
    }
}