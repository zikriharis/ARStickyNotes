using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StickyNote : MonoBehaviour
{
    public TMP_InputField noteInput;
    public Toggle completeToggle;

    public Button moreButton;
    public GameObject optionsPanel;

    public Button editButton;
    public Button deleteButton;
    public Button reminderButton;

    private bool isEditing = false;

    void Start()
    {
        // Hide options panel by default
        optionsPanel.SetActive(false);

        moreButton.onClick.AddListener(ToggleOptionsMenu);
        editButton.onClick.AddListener(ToggleEdit);
        deleteButton.onClick.AddListener(DeleteNote);
        reminderButton.onClick.AddListener(SetReminder);

        noteInput.interactable = false;
    }

    void ToggleOptionsMenu()
    {
        optionsPanel.SetActive(!optionsPanel.activeSelf);
    }

    void ToggleEdit()
    {
        isEditing = !isEditing;
        noteInput.interactable = isEditing;
        optionsPanel.SetActive(false); // Optional: auto-hide
    }

    void DeleteNote()
    {
        Destroy(gameObject);
    }

    void SetReminder()
    {
        Debug.Log("Reminder triggered!");
        optionsPanel.SetActive(false); // Optional: auto-hide
    }
}
