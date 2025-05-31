using System.Collections.Generic;
using UnityEngine;

public class StickyNoteShowAllToggle : MonoBehaviour
{
    public static StickyNoteShowAllToggle Instance { get; private set; }
    public List<StickyNoteHideShow> stickyNotes = new List<StickyNoteHideShow>();
    public MonoBehaviour showAllToggle;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (showAllToggle != null)
        {
            var eventField = showAllToggle.GetType().GetField("onValueChanged");
            if (eventField != null)
            {
                var evt = eventField.GetValue(showAllToggle) as UnityEngine.Events.UnityEvent<bool>;
                if (evt != null)
                {
                    evt.AddListener(OnShowAllToggleChanged);
                    var valueProp = showAllToggle.GetType().GetProperty("Value");
                    if (valueProp != null)
                    {
                        bool value = (bool)valueProp.GetValue(showAllToggle);
                        OnShowAllToggleChanged(value);
                    }
                }
            }
        }
    }

    public void RegisterStickyNote(StickyNoteHideShow note)
    {
        if (!stickyNotes.Contains(note))
            stickyNotes.Add(note);
    }
    public void UnregisterStickyNote(StickyNoteHideShow note)
    {
        stickyNotes.Remove(note);
    }

    public void OnShowAllToggleChanged(bool isShowAll)
    {
        foreach (var note in stickyNotes)
        {
            if (note == null) continue;
            if (isShowAll)
                note.EnterShowAllMode();
            else
                note.ExitShowAllMode();
        }
    }
}