using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    public Tooltip tooltip;

    #region Singleton
    public static TooltipSystem instance;
    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    #endregion

    public static void Show(string content, string header = "")
    {
        instance.tooltip.gameObject.SetActive(true);
        instance.tooltip.SetText(content, header);
    }

    public static void Hide()
    {
        instance.tooltip.gameObject.SetActive(false);
    }
}
