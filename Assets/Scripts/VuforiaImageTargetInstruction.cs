using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Vuforia;

public class VuforiaImageTargetInstruction : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup instructionCanvasGroup; // Assign your Panel's CanvasGroup
    public TMP_Text instructionText;           // Assign your TMP_Text or Text component

    [Header("Instruction Message")]
    [TextArea]
    public string message = "No image target detected.\nPlease move your camera or scan the target image.\nMake sure the room is well lit.";

    [Header("Fade Settings")]
    public float fadeDuration = 0.3f;

    private bool isShowing = false;

    void Start()
    {
        instructionText.text = message;
        ShowInstruction();
        // Register to all ObserverBehaviour status changes using the new API
        foreach (var observer in Object.FindObjectsByType<ObserverBehaviour>(FindObjectsSortMode.None))
        {
            observer.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    void OnDestroy()
    {
        foreach (var observer in Object.FindObjectsByType<ObserverBehaviour>(FindObjectsSortMode.None))
        {
            observer.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        bool anyTracked = false;
        // Check ALL image targets and see if ANY are tracked
        foreach (var observer in Object.FindObjectsByType<ObserverBehaviour>(FindObjectsSortMode.None))
        {
            var s = observer.TargetStatus.Status;
            if (s == Status.TRACKED || s == Status.EXTENDED_TRACKED)
            {
                anyTracked = true;
                break;
            }
        }
        if (anyTracked)
        {
            if (isShowing)
                StartCoroutine(FadeOut());
        }
        else
        {
            if (!isShowing)
                StartCoroutine(FadeIn());
        }
    }

    private void ShowInstruction()
    {
        instructionCanvasGroup.alpha = 1f;
        instructionCanvasGroup.blocksRaycasts = true;
        instructionCanvasGroup.interactable = true;
        isShowing = true;
    }

    private System.Collections.IEnumerator FadeIn()
    {
        isShowing = true;
        instructionCanvasGroup.blocksRaycasts = true;
        instructionCanvasGroup.interactable = true;
        float start = instructionCanvasGroup.alpha;
        float t = 0;
        while (t < fadeDuration)
        {
            instructionCanvasGroup.alpha = Mathf.Lerp(start, 1f, t / fadeDuration);
            t += Time.deltaTime;
            yield return null;
        }
        instructionCanvasGroup.alpha = 1f;
    }

    private System.Collections.IEnumerator FadeOut()
    {
        isShowing = false;
        instructionCanvasGroup.blocksRaycasts = false;
        instructionCanvasGroup.interactable = false;
        float start = instructionCanvasGroup.alpha;
        float t = 0;
        while (t < fadeDuration)
        {
            instructionCanvasGroup.alpha = Mathf.Lerp(start, 0f, t / fadeDuration);
            t += Time.deltaTime;
            yield return null;
        }
        instructionCanvasGroup.alpha = 0f;
    }
}