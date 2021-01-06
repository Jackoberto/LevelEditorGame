using UnityEngine;
using UnityEngine.UI;

public class ScaleRect : MonoBehaviour
{
    public float multiplier;

    [ContextMenu("MultiplyAllRects")]
    public void MultiplyAllRects()
    {
        var rects = GetComponentsInChildren<RectTransform>();
        foreach (var rectTransform in rects)
        {
            var newSize = rectTransform.sizeDelta * multiplier;
            var newPos = rectTransform.anchoredPosition * multiplier;
            rectTransform.sizeDelta = newSize;
            rectTransform.anchoredPosition = newPos;
        }

        var texts = GetComponentsInChildren<Text>();

        foreach (var text in texts)
        {
            text.fontSize = Mathf.RoundToInt(text.fontSize * multiplier);
        }
    }
}
