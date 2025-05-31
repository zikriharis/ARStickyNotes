using UnityEngine;
using UnityEngine.EventSystems;
using Lean.Gui;

public class MoveParentToTopOnPointerDown : MonoBehaviour, IPointerDownHandler
{
    public LeanMoveToTop moveToTop; // Assign in inspector (Root's LeanMoveToTop)

    public void OnPointerDown(PointerEventData eventData)
    {
        if (moveToTop != null)
        {
            moveToTop.OnPointerDown(eventData);
        }
    }
}