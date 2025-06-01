using UnityEngine;
using UnityEngine.UI;

public class StickyNoteUIExpander : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public GameObject actionButtonsPanel; // Parent of all your sticky note buttons except expand
    public Button expandButton;
    public Image expandIcon; // Optional: icon to flip for expand/collapse
    public Sprite expandSprite; // Optional: icon when collapsed
    public Sprite collapseSprite; // Optional: icon when expanded

    private bool isExpanded = false;

    void Awake()
    {
        // Hide action buttons by default
        SetExpanded(false);
        expandButton.onClick.AddListener(ToggleExpand);
    }

    void OnDestroy()
    {
        expandButton.onClick.RemoveListener(ToggleExpand);
    }

    public void ToggleExpand()
    {
        SetExpanded(!isExpanded);
    }

    private void SetExpanded(bool expanded)
    {
        isExpanded = expanded;
        if (actionButtonsPanel != null)
            actionButtonsPanel.SetActive(expanded);

        // Optional: swap icon
        if (expandIcon != null)
        {
            expandIcon.sprite = expanded ? collapseSprite : expandSprite;
            expandIcon.transform.localEulerAngles = expanded ? new Vector3(0, 0, 180) : Vector3.zero;
        }
    }
}