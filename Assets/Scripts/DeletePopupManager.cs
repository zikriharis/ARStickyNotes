using UnityEngine;
using UnityEngine.UI;

public class DeletePopupManager : MonoBehaviour
{
    public GameObject popupPanel;
    public Button confirmButton;
    public Button cancelButton;

    [Header("Reference to the board script for tap cooldown")]
    public PinchToScaleAndTapToSpawn boardScript; // Assign this in inspector or at runtime

    private GameObject targetToDelete;

    public void ShowPopup(GameObject target)
    {
        targetToDelete = target;
        popupPanel.SetActive(true);
    }

    private void Start()
    {
        popupPanel.SetActive(false);
        confirmButton.onClick.AddListener(() =>
        {
            if (targetToDelete != null)
            {
                Destroy(targetToDelete);
                targetToDelete = null;
            }
            popupPanel.SetActive(false);

            // Notify board to start tap cooldown
            if (boardScript != null)
                boardScript.OnStickyNoteHideOrPopup();
        });

        cancelButton.onClick.AddListener(() =>
        {
            targetToDelete = null;
            popupPanel.SetActive(false);

            // Notify board to start tap cooldown
            if (boardScript != null)
                boardScript.OnStickyNoteHideOrPopup();
        });
    }
}