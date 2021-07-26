using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System;
public class WaveTransition : MonoBehaviour
{
    public Color transitionColor;
    RectTransform rTransform;
    TextMeshProUGUI tmp;
    public delegate void Callback();
    void Awake()
    {
        rTransform = GetComponent<RectTransform>();
        tmp = GetComponent<TextMeshProUGUI>();
    }

    public void EndWave(Callback c)
    {
        rTransform.anchoredPosition = new Vector3(0, -700, 0);

        tmp.text = "Wave Complete!";

        Sequence slideIn = DOTween.Sequence();
        slideIn.Append(rTransform.DOAnchorPosY(0, .4f).SetEase(Ease.OutBounce));
        slideIn.AppendInterval(1f);
        slideIn.Append(rTransform.DOAnchorPosY(650, .25f));
        slideIn.AppendCallback(()=> { c?.Invoke(); });
    }

    public void StartWave(Callback c)
    {
        rTransform.anchoredPosition = new Vector3(0, -700, 0);

        tmp.text = "Wave Start!";

        Sequence slideIn = DOTween.Sequence();
        slideIn.Append(rTransform.DOAnchorPosY(0, .4f).SetEase(Ease.OutBounce));
        slideIn.AppendInterval(1f);
        slideIn.Append(rTransform.DOAnchorPosY(650, .25f));
        slideIn.AppendCallback(() => { c?.Invoke(); });
    }
    public void Transition(string transition, Callback c)
    {
        Time.timeScale = 0;
        rTransform.anchoredPosition = new Vector3(0, -700, 0);

        tmp.text = transition;
        tmp.color = transitionColor;

        Sequence slideIn = DOTween.Sequence().SetUpdate(true);
        slideIn.Append(rTransform.DOAnchorPosY(0, .4f).SetEase(Ease.OutBounce)).SetUpdate(true);
        slideIn.AppendInterval(1f).SetUpdate(true);
        slideIn.Append(rTransform.DOAnchorPosY(650, .25f)).SetUpdate(true);
        slideIn.AppendCallback(() => { c?.Invoke(); }).SetUpdate(true);
    }
}
