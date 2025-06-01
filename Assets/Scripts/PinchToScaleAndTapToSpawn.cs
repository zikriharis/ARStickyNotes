using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Attach this script to your 3D board GameObject.
/// - Pinch to scale the board (with boundary logic to keep notes on the board and margin).
/// - Tap-and-hold to spawn sticky notes, clamping so notes never spawn out of bounds (with margin).
/// - Sticky notes are draggable only within the board boundary (with margin), and move to top when dragging.
/// </summary>
public class PinchToScaleAndTapToSpawn : MonoBehaviour
{
    [Header("Board Scaling")]
    public float minScale = 0.2f;
    public float maxScale = 2.0f;

    [Header("Sticky Note Spawn Settings")]
    [Tooltip("Prefab for UI sticky notes (root must be RectTransform, no Canvas inside).")]
    public GameObject stickyNotePrefab;
    [Tooltip("Reference to the World Space Canvas (RectTransform) overlaying the board.")]
    public RectTransform stickyNotesCanvas;
    [Tooltip("Default localScale for spawned sticky notes (try 0.2, 0.2, 0.2 for small notes).")]
    public Vector3 stickyNoteSpawnScale = Vector3.one;
    [Tooltip("Default localRotation (Euler angles) for spawned sticky notes.")]
    public Vector3 stickyNoteSpawnRotation = Vector3.zero;

    [Header("Tap and Hold")]
    public float hideCooldown = 1.0f;
    public float spawnHoldDuration = 0.5f;

    [Header("Sticky Note Margin")]
    [Tooltip("Margin to keep sticky notes away from board edge during resize, spawn, and dragging.")]
    public float stickyNoteMargin = 0.02f;

    // Private state
    private float initialDistance;
    private Vector3 initialScale;
    private bool isScaling = false;

    private Camera mainCamera;
    private Outline outline;

    private float lastHideTime = -10f;
    private bool isTouching = false;
    private float touchStartTime = 0f;

    void Start()
    {
        mainCamera = Camera.main;
        outline = GetComponent<Outline>();
        UpdateCanvasSize();
        if (stickyNotesCanvas != null)
            stickyNotesCanvas.localScale = Vector3.one;
    }

    void Update()
    {
        // --- Pinch to Scale (with sticky note boundary enforcement) ---
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            if (t1.phase == TouchPhase.Began)
            {
                if (RaycastToThisObject(t0.position) || RaycastToThisObject(t1.position))
                {
                    isScaling = true;
                    initialDistance = Vector2.Distance(t0.position, t1.position);
                    initialScale = transform.localScale;
                    if (outline != null) outline.EnableOutline();
                }
            }

            if (isScaling && (t0.phase == TouchPhase.Moved || t1.phase == TouchPhase.Moved))
            {
                float currentDistance = Vector2.Distance(t0.position, t1.position);
                if (Mathf.Approximately(initialDistance, 0)) return;

                float scaleFactor = currentDistance / initialDistance;
                float targetX = Mathf.Clamp(initialScale.x * scaleFactor, minScale, maxScale);
                float xzRatio = initialScale.z / initialScale.x;
                float targetZ = targetX * xzRatio;

                // Limit scale so all sticky notes stay in bounds (with margin)
                Vector2 boardSize = new Vector2(targetX, targetZ);
                if (WouldAnyStickyNoteBeOutOfBounds(boardSize))
                {
                    // Don't allow scaling further down
                    return;
                }

                transform.localScale = new Vector3(targetX, initialScale.y, targetZ);
                UpdateCanvasSize();
                if (stickyNotesCanvas != null)
                    stickyNotesCanvas.localScale = Vector3.one;
            }
        }
        else if (isScaling)
        {
            isScaling = false;
            if (outline != null) outline.DisableOutline();
        }

        // --- Tap-and-hold to spawn sticky note (with clamping) ---
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (Time.time - lastHideTime < hideCooldown)
                return;

            if (IsPointerOverUI(touch))
                return;

            if (touch.phase == TouchPhase.Began)
            {
                isTouching = true;
                touchStartTime = Time.time;
            }
            else if (touch.phase == TouchPhase.Ended && isTouching)
            {
                float heldTime = Time.time - touchStartTime;
                isTouching = false;

                if (heldTime >= spawnHoldDuration)
                {
                    Ray ray = mainCamera.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        if (hit.transform == transform)
                        {
                            SpawnStickyNoteAtBoardPoint(hit.point);
                        }
                    }
                }
            }
        }
        else
        {
            isTouching = false;
        }
    }

    private bool RaycastToThisObject(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.transform == transform;
        }
        return false;
    }

    private bool IsPointerOverUI(Touch touch)
    {
        if (EventSystem.current != null)
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            return EventSystem.current.IsPointerOverGameObject(touch.fingerId);
#else
            return EventSystem.current.IsPointerOverGameObject();
#endif
        }
        return false;
    }

    /// <summary>
    /// Returns true if any sticky note would be out of bounds at the given board size, considering the margin.
    /// </summary>
    private bool WouldAnyStickyNoteBeOutOfBounds(Vector2 boardSize)
    {
        foreach (RectTransform note in GetAllStickyNotes())
        {
            Vector2 center = note.anchoredPosition;
            Vector2 halfSize = Vector2.Scale(note.rect.size, note.localScale) * 0.5f;

            // Subtract margin from allowed board area
            float allowedHalfWidth = (boardSize.x / 2f) - stickyNoteMargin;
            float allowedHalfHeight = (boardSize.y / 2f) - stickyNoteMargin;

            if (Mathf.Abs(center.x) + halfSize.x > allowedHalfWidth ||
                Mathf.Abs(center.y) + halfSize.y > allowedHalfHeight)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Gets all sticky notes as RectTransforms (children of the canvas).
    /// </summary>
    private IEnumerable<RectTransform> GetAllStickyNotes()
    {
        foreach (Transform child in stickyNotesCanvas)
        {
            RectTransform note = child.GetComponent<RectTransform>();
            if (note != null) yield return note;
        }
    }

    /// <summary>
    /// Spawns a sticky note as a UI element at the given point on the board,
    /// clamping the position so it stays on the board with margin.
    /// </summary>
    private void SpawnStickyNoteAtBoardPoint(Vector3 worldPosition)
    {
        if (stickyNotePrefab == null || stickyNotesCanvas == null)
        {
            Debug.LogWarning("stickyNotePrefab or stickyNotesCanvas is not assigned!");
            return;
        }

        Vector2 canvasLocalPos = WorldToCanvasLocal(worldPosition);

        // Clamp so sticky note is fully inside the board, with margin
        RectTransform prefabRect = stickyNotePrefab.GetComponent<RectTransform>();
        Vector2 halfBoard = new Vector2(stickyNotesCanvas.rect.width, stickyNotesCanvas.rect.height) * 0.5f;
        Vector2 halfNote = Vector2.Scale(prefabRect.rect.size, stickyNoteSpawnScale) * 0.5f;
        float allowedHalfWidth = halfBoard.x - halfNote.x - stickyNoteMargin;
        float allowedHalfHeight = halfBoard.y - halfNote.y - stickyNoteMargin;

        canvasLocalPos.x = Mathf.Clamp(canvasLocalPos.x, -allowedHalfWidth, allowedHalfWidth);
        canvasLocalPos.y = Mathf.Clamp(canvasLocalPos.y, -allowedHalfHeight, allowedHalfHeight);

        GameObject stickyNote = Instantiate(stickyNotePrefab, stickyNotesCanvas);
        RectTransform noteRect = stickyNote.GetComponent<RectTransform>();
        noteRect.anchoredPosition = canvasLocalPos;
        noteRect.localRotation = Quaternion.Euler(stickyNoteSpawnRotation);
        noteRect.localScale = stickyNoteSpawnScale;

        // Add drag handler if not already present
        if (stickyNote.GetComponent<StickyNoteDragHandler>() == null)
            stickyNote.AddComponent<StickyNoteDragHandler>().Init(stickyNotesCanvas, stickyNoteMargin);

        // Optional: assign reference to board script, if your sticky notes use it
        var stickyScript = stickyNote.GetComponent<StickyNoteHideShow>();
        if (stickyScript != null)
            stickyScript.boardScript = this;
    }

    /// <summary>
    /// Converts a world position (on the board) to a local position on the sticky notes Canvas.
    /// </summary>
    private Vector2 WorldToCanvasLocal(Vector3 worldPos)
    {
        Vector3 local = stickyNotesCanvas.InverseTransformPoint(worldPos);
        return new Vector2(local.x, local.y);
    }

    /// <summary>
    /// Keeps the Canvas's width/height matching the board's X/Z scale (but scale is always Vector3.one!).
    /// </summary>
    private void UpdateCanvasSize()
    {
        if (stickyNotesCanvas == null) return;
        Vector3 boardScale = transform.localScale;
        stickyNotesCanvas.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, boardScale.x);
        stickyNotesCanvas.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, boardScale.z);
        stickyNotesCanvas.localScale = Vector3.one;
    }

    public void OnStickyNoteHideOrPopup()
    {
        lastHideTime = Time.time;
    }
}

/// <summary>
/// Drag handler for sticky notes: restricts dragging to within the board boundary (with margin), moves note to top while dragging, and shows an outline while dragging.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class StickyNoteDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform noteRect;
    private RectTransform boardRect;
    private Vector2 offset;
    private float margin = 0.02f;
    private StickyNoteOutline outline;

    public void Init(RectTransform boardCanvasRect, float stickyNoteMargin)
    {
        boardRect = boardCanvasRect;
        margin = stickyNoteMargin;
        // Add StickyNoteOutline if not already present
        outline = GetComponent<StickyNoteOutline>();
        if (outline == null)
            outline = gameObject.AddComponent<StickyNoteOutline>();
        outline.enabled = false;
    }

    void Awake()
    {
        noteRect = GetComponent<RectTransform>();
        outline = GetComponent<StickyNoteOutline>();
        if (outline != null)
            outline.enabled = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (noteRect == null) noteRect = GetComponent<RectTransform>();
        noteRect.SetAsLastSibling();

        // Calculate pointer offset from note center
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(noteRect, eventData.position, eventData.pressEventCamera, out localPoint);
        offset = noteRect.anchoredPosition - localPoint;

        // Enable outline during drag
        if (outline == null)
            outline = gameObject.AddComponent<StickyNoteOutline>();
        outline.enabled = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(boardRect, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            Vector2 newAnchoredPos = localPoint + offset;

            // Clamp so sticky note stays on board, with margin
            Vector2 halfBoard = new Vector2(boardRect.rect.width, boardRect.rect.height) * 0.5f;
            Vector2 halfNote = Vector2.Scale(noteRect.rect.size, noteRect.localScale) * 0.5f;
            float allowedHalfWidth = halfBoard.x - halfNote.x - margin;
            float allowedHalfHeight = halfBoard.y - halfNote.y - margin;

            newAnchoredPos.x = Mathf.Clamp(newAnchoredPos.x, -allowedHalfWidth, allowedHalfWidth);
            newAnchoredPos.y = Mathf.Clamp(newAnchoredPos.y, -allowedHalfHeight, allowedHalfHeight);

            noteRect.anchoredPosition = newAnchoredPos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Disable outline after drag
        if (outline != null)
            outline.enabled = false;
    }
}