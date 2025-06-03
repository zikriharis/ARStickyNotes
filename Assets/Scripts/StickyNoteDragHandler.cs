using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Attach this script to your sticky note prefab.
/// Handles dragging within a board, moves sticky note to top on drag,
/// keeps it within board bounds (with margin), and shows outline during drag.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class StickyNoteDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform noteRect;
    private RectTransform boardRect;
    private float margin = 0.02f;
    private Vector2 offset;
    private StickyNoteOutline outline;

    /// <summary>
    /// Call this after instantiating the sticky note to initialize board reference and margin.
    /// </summary>
    public void Init(RectTransform boardCanvasRect, float stickyNoteMargin)
    {
        boardRect = boardCanvasRect;
        margin = stickyNoteMargin;
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
        RectTransformUtility.ScreenPointToLocalPointInRectangle(boardRect, eventData.position, eventData.pressEventCamera, out localPoint);
        offset = noteRect.anchoredPosition - localPoint;

        // Enable outline during drag
        if (outline == null)
            outline = gameObject.AddComponent<StickyNoteOutline>();
        outline.enabled = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (boardRect == null) return;
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