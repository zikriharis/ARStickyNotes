using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UISwitcher
{
    public class UISwitcher : UINullableToggle
    {
        private readonly Vector2 _min = new(0, 0.5f);
        private readonly Vector2 _max = new(1, 0.5f);
        private readonly Vector2 _middle = new(0.5f, 0.5f);

        [SerializeField] private Graphic backgroundGraphic;
        [SerializeField] private Color onColor, offColor, nullColor;
        [SerializeField] private RectTransform tipRect;
        [SerializeField] private TextMeshProUGUI priorityText;

        private Color backgroundColor
        {
            set
            {
                if (backgroundGraphic == null) return;
                backgroundGraphic.color = value;
            }
        }
        protected override void OnChanged(bool? obj)
        {
            if (obj.HasValue)
            {
                if (obj.Value)
                    SetOn();
                else
                    SetOff();
            }
            else
            {
                SetNull();
            }
        }

        private void SetOn()
        {
            SetAnchors(_max);
            backgroundColor = onColor;
            if (priorityText != null)
                priorityText.text = "High Priority"; // Set text
        }

        private void SetOff()
        {
            SetAnchors(_min);
            backgroundColor = offColor;
            if (priorityText != null)
                priorityText.text = "Low Priority"; // Set text
        }

        private void SetNull()
        {
            SetAnchors(_middle);
            backgroundColor = nullColor;
            if (priorityText != null)
                priorityText.text = "Medium Priority"; // Set text
        }

        private void SetAnchors(Vector2 anchor)
        {
            tipRect.anchorMin = anchor;
            tipRect.anchorMax = anchor;
            tipRect.pivot = anchor;
        }
    }
}