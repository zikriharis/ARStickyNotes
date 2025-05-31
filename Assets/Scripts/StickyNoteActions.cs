using UnityEngine;

public class StickyNoteActions : MonoBehaviour
{
    public void RequestDelete()
    {
        // Call the popup and pass this sticky note's GameObject
        DeletePopupManager popup = Object.FindFirstObjectByType<DeletePopupManager>();
        if (popup != null)
        {
            popup.ShowPopup(gameObject);
        }
        else
        {
            Debug.LogWarning("DeletePopupManager not found in scene.");
        }
    }
}
