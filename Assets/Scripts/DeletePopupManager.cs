using UnityEngine;
using UnityEngine.UI;

public class DeletePopupManager : MonoBehaviour
{
    public GameObject popupPanel;
    public Button confirmButton;
    public Button cancelButton;

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
        });

        cancelButton.onClick.AddListener(() =>
        {
            targetToDelete = null;
            popupPanel.SetActive(false);
        });
    }
}
