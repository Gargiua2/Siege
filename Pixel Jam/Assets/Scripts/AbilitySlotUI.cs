using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AbilitySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] Image outline;
    [SerializeField] Image icon;
    [SerializeField] ProgressBar chargeBar;

    public AbilityTabs manager;

    public void Intialize(Ability a, AbilityTabs m)
    {
        manager = m;
        icon.sprite = a.icon;
        Controller.instance.selectedUnit.abilities[transform.GetSiblingIndex()].ui = this;
        TooltipTrigger tooltip = GetComponent<TooltipTrigger>();
        tooltip.header = a.name;
        tooltip.body = a.description;                                                                               
    }

    public void UpdateCharge(float a)
    {
        chargeBar.SetProgress(a);
    }

    public void Unselect()
    {
        outline.enabled = false;
    }
    public void Select()
    {
        outline.enabled = true;
        outline.sprite = manager.selectionBorder;
        manager.selected = this;
        Controller.instance.selectedAbilitySlot = transform.GetSiblingIndex();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (manager.selected != this)
        {
            outline.enabled = true;
            outline.sprite = manager.hoverBorder;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (manager.selected != this)
        {
            outline.enabled = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (manager.selected != this)
        {
            manager.UnselectAll();
            Select();
        }
    }
}
