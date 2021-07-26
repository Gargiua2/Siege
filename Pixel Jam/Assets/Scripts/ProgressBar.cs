using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    RectTransform rectTransform;
    float maxLength;
    Image bar;
    [SerializeField] Color full;
    [SerializeField] Color empty;

    private void Awake()
    {
        maxLength = (transform.parent as RectTransform).rect.width;
        rectTransform = transform as RectTransform;
        bar = GetComponent<Image>();
    }

    public void SetProgress(float a)
    {
        a = Mathf.Clamp01(a);

        bar.color = Color.Lerp(empty, full, a);

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(0, maxLength, a));
    }
}
