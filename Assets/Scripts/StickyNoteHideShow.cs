using UnityEngine;
using UnityEngine.UI;

public class StickyNoteHideShow : MonoBehaviour
{
    [Header("Assign these in the Inspector (or auto-assigned at runtime):")]
    public MonoBehaviour hideToggle;      // Assign your UISwitcherComplete component here
    public Image noteBackground;          // The main background image for greying out
    public GameObject stickyNoteRoot;     // The root GameObject for the sticky note
    public GameObject colorPaletteRoot;

    [Header("Greyed out color for Show All mode")]
    public Color greyedOutColor = new Color(0.7f, 0.7f, 0.7f, 1f);

    [Header("Reference to board script for tap cooldown")]
    public PinchToScaleAndTapToSpawn boardScript;

    private Color assignedColor;
    private bool isHidden = false;
    private bool isShowAllMode = false;

    void Awake()
    {
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

        // Initialize assignedColor to the initial color of the note background (as set in Inspector or prefab)
        if (noteBackground != null)
            assignedColor = noteBackground.color;
    }

    void Start()
    {
        // Subscribe to the Boolean event of your custom toggle component
        if (hideToggle != null)
        {
            var eventField = hideToggle.GetType().GetField("onValueChanged");
            if (eventField != null)
            {
                var evt = eventField.GetValue(hideToggle) as UnityEngine.Events.UnityEvent<bool>;
                if (evt != null)
                {
                    evt.AddListener(OnHideToggleChanged);
                    // Also set initial state
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

        // Register this sticky note with the ShowAll manager
        if (StickyNoteShowAllToggle.Instance != null)
            StickyNoteShowAllToggle.Instance.RegisterStickyNote(this);
    }

    void OnDestroy()
    {
        // Unregister from ShowAll manager
        if (StickyNoteShowAllToggle.Instance != null)
            StickyNoteShowAllToggle.Instance.UnregisterStickyNote(this);
    }

    public void OnHideToggleChanged(bool isOn)
    {
        isHidden = isOn;
        UpdateVisual();

        if (boardScript != null)
            boardScript.OnStickyNoteHideOrPopup();
    }

    /// <summary>
    /// Call this when the user picks a new color for the note (e.g., from a color picker UI).
    /// This sets the persistent color and updates the UI.
    /// </summary>
    public void SetNoteColor(Color color)
    {
        assignedColor = color;
        UpdateVisual();
    }

    /// <summary>
    /// Call this to update the visual appearance of the note, including color.
    /// Always uses assignedColor except when greyed out for show-all mode.
    /// </summary>
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

    public void OnPopupClosed()
    {
        if (boardScript != null)
            boardScript.OnStickyNoteHideOrPopup();
    }
}