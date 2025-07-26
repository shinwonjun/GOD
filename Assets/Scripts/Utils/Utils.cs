using TMPro;
using UnityEngine;

public static class Utils
{
    public static void CopyRectTransform(RectTransform source, RectTransform target)
    {
        target.anchorMin = source.anchorMin;
        target.anchorMax = source.anchorMax;
        target.anchoredPosition = source.anchoredPosition;
        target.sizeDelta = source.sizeDelta;
        target.pivot = source.pivot;
        target.localScale = source.localScale;
        target.localRotation = source.localRotation;
        target.localPosition = source.localPosition;
    }
}
