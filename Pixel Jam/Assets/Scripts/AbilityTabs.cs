using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTabs : MonoBehaviour
{
    public GameObject slotPrefab;
    public List<AbilitySlotUI> slots = new List<AbilitySlotUI>();

    public AbilitySlotUI selected = null;

    public Sprite selectionBorder;
    public Sprite hoverBorder;

    public void Update()
    {
        int num = GetPressedNumber()-1;
        
        if(num > -1 && num < slots.Count)
        {
            UnselectAll();
            slots[num].Select();
        }
    }

    public int GetPressedNumber()
    {
        for (int number = 0; number <= 9; number++)
        {
            if (Input.GetKeyDown(number.ToString()))
                return number;
        }

        return -1;
    }

    public void Initialize(List<Ability> abilities)
    {
        for(int i = transform.childCount-1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        slots = new List<AbilitySlotUI>();

        for(int i = 0; i < abilities.Count; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab);
            newSlot.transform.SetParent(transform,false);
            AbilitySlotUI slot = newSlot.GetComponent<AbilitySlotUI>();
            slots.Add(slot);
            slot.Intialize(abilities[i], this);
        }

        UnselectAll();
        slots[0].Select();
    }

    public void UnselectAll()
    {
        for(int i = 0; i < slots.Count; i++)
        {
            slots[i].Unselect();
        }
    }
}
