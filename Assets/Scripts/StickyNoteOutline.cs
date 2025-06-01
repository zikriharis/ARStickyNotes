using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple outline effect for sticky notes. Attach to the sticky note root or relevant child with a RectTransform.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class StickyNoteOutline : MonoBehaviour
{
    public Color outlineColor = Color.yellow;
    public float outlineWidth = 4f;

    private GameObject outlineObj;
    private Image outlineImg;

    void OnEnable()
    {
        CreateOrEnableOutline();
    }

    void OnDisable()
    {
        if (outlineObj != null)
            outlineObj.SetActive(false);
    }

    void OnDestroy()
    {
        if (outlineObj != null)
            Destroy(outlineObj);
    }

    void CreateOrEnableOutline()
    {
        if (outlineObj == null)
        {
            outlineObj = new GameObject("Outline");
            outlineObj.transform.SetParent(transform, false);
            outlineObj.transform.SetAsFirstSibling();

            outlineImg = outlineObj.AddComponent<Image>();
            outlineImg.raycastTarget = false;
            outlineImg.color = outlineColor;

            // Set as transparent
            outlineImg.sprite = null; // fallback to default

            RectTransform outlineRect = outlineObj.GetComponent<RectTransform>();
            RectTransform thisRect = GetComponent<RectTransform>();
            outlineRect.anchorMin = Vector2.zero;
            outlineRect.anchorMax = Vector2.one;
            outlineRect.pivot = thisRect.pivot;
            outlineRect.sizeDelta = new Vector2(outlineWidth * 2, outlineWidth * 2);
            outlineRect.anchoredPosition = Vector2.zero;
        }
        else
        {
            outlineObj.SetActive(true);
        }
        // Update color and size in case properties changed
        if (outlineImg != null)
        {
            outlineImg.color = outlineColor;
            outlineImg.raycastTarget = false;
        }
        RectTransform outlineRect2 = outlineObj.GetComponent<RectTransform>();
        RectTransform thisRect2 = GetComponent<RectTransform>();
        outlineRect2.sizeDelta = new Vector2(outlineWidth * 2, outlineWidth * 2);
        outlineRect2.anchoredPosition = Vector2.zero;
    }
}