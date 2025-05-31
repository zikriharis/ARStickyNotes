using UnityEngine;
using UnityEngine.UI;

public class StickyNoteHideShow : MonoBehaviour
{
    [Header("Assign these in the Inspector (or auto-assigned at runtime):")]
    public MonoBehaviour hideToggle;      // Assign your UISwitcherComplete component here
    public Image noteBackground;          // The main background image for greying out
    public GameObject stickyNoteRoot;     // The root GameObject for the sticky note

    [Header("Greyed out color for Show All mode")]
    public Color greyedOutColor = new Color(0.7f, 0.7f, 0.7f, 1f);

    [Header("Reference to board script for tap cooldown")]
    public PinchToScaleAndTapToSpawn boardScript; // <-- Add this

    private Color assignedColor = Color.yellow;
    private bool isHidden = false;
    private bool isShowAllMode = false;

    void Awake()
    {
        // ... (same as before, no changes needed) ...
        if (hideToggle == null)
        {
            Transform showHideToggle = transform.Find("Canvas/Show/Hide Toggle");
            if (showHideToggle != null)
                hideToggle = showHideToggle.GetComponent<MonoBehaviour>();
            if (hideToggle == null)
                hideToggle = GetComponentInChildren<MonoBehaviour>(true);
        }

        if (noteBackground == null)
        {
            Transform bg = transform.Find("Canvas/NoteBackground");
            if (bg != null)
                noteBackground = bg.GetComponent<Image>();
            if (noteBackground == null)
                noteBackground = GetComponentInChildren<Image>(true);
        }

        if (stickyNoteRoot == null)
            stickyNoteRoot = this.gameObject;
    }

    void Start()
    {
        // ... (same as before) ...
        if (hideToggle != null)
        {
            var eventField = hideToggle.GetType().GetField("onValueChanged");
            if (eventField != null)
            {
                var evt = eventField.GetValue(hideToggle) as UnityEngine.Events.UnityEvent<bool>;
                if (evt != null)
                {
                    evt.AddListener(OnHideToggleChanged);
                    var valueProp = hideToggle.GetType().GetProperty("Value");
                    if (valueProp != null)
                    {
                        bool value = (bool)valueProp.GetValue(hideToggle);
                        OnHideToggleChanged(value);
                    }
                }
                else
                {
                    Debug.LogWarning("StickyNoteHideShow: Could not find UnityEvent<bool> onValueChanged on hideToggle.");
                }
            }
            else
            {
                Debug.LogWarning("StickyNoteHideShow: Could not find onValueChanged field on hideToggle.");
            }
        }

        if (noteBackground != null)
            assignedColor = noteBackground.color;
    }

    public void OnHideToggleChanged(bool isOn)
    {
        isHidden = isOn;
        UpdateVisual();

        // --- ADD THIS: Notify board to start cooldown ---
        if (boardScript != null)
            boardScript.OnStickyNoteHideOrPopup();
    }

    public void SetNoteColor(Color color)
    {
        assignedColor = color;
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if (isShowAllMode)
        {
            stickyNoteRoot.SetActive(true);
            if (noteBackground != null)
                noteBackground.color = isHidden ? greyedOutColor : assignedColor;
            SetAllUIInteractable(true);
        }
        else
        {
            stickyNoteRoot.SetActive(!isHidden);
            if (noteBackground != null && !isHidden)
                noteBackground.color = assignedColor;
            SetAllUIInteractable(!isHidden);
        }
    }

    public void EnterShowAllMode()
    {
        isShowAllMode = true;
        UpdateVisual();
    }

    public void ExitShowAllMode()
    {
        isShowAllMode = false;
        UpdateVisual();
    }

    public bool IsHidden() => isHidden;

    private void SetAllUIInteractable(bool interactable)
    {
        foreach (Selectable ui in stickyNoteRoot.GetComponentsInChildren<Selectable>(true))
        {
            ui.interactable = interactable;
        }
    }

    // --- ADD THIS: Call from popup confirmation/cancel buttons ---
    public void OnPopupClosed()
    {
        if (boardScript != null)
            boardScript.OnStickyNoteHideOrPopup();
    }
}