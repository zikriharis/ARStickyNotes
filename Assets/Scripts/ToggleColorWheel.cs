using UnityEngine;

public class ToggleColorWheel : MonoBehaviour
{
    public GameObject colorWheelPanel;

    public void TogglePanel()
    {
        colorWheelPanel.SetActive(!colorWheelPanel.activeSelf);
    }

    public void HidePanel()
    {
        colorWheelPanel.SetActive(false);
    }
}
