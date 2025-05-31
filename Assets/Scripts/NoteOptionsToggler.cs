using UnityEngine;

public class NoteOptionsToggler : MonoBehaviour
{
    public GameObject optionsPanel;

    public void ToggleOptionsPanel()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(!optionsPanel.activeSelf);
        }
    }
}
