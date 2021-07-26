using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{
    public string header;
    [TextArea(1,4)]public string body;

    public Sequence s;

    bool active = false; 

    public void OnPointerEnter(PointerEventData eventData)
    {
        active = true;
        s = DOTween.Sequence();
        s.AppendInterval(.4f);
        s.AppendCallback(() => { TooltipSystem.Show(body, header); });
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        active = false;
        s.Kill();
        TooltipSystem.Hide();
    }

    public void OnDisable()
    {
        if (active)
        {
            active = false;
            s.Kill();
            TooltipSystem.Hide();
        }
    }
    public void OnDestroy()
    {
        if (active)
        {
            active = false;
            s.Kill();
            TooltipSystem.Hide();
        }
    }
}
