using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public PlayerUnit defaultSelection;
    public PlayerUnit selectedUnit;
    public List<PlayerUnit> units;
    int selectedIndex = 0;
    public int selectedAbilitySlot;

    public Vector2 minSelectableAreaBounds;
    public Vector2 maxSelectableAreaBounds;

    AbilityTabs tabs;

    #region Singleton
    public static Controller instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion

     void Start()
    {
        for (int i = 0; i < units.Count; i++)
        {
            units[i].unitIndex = i;
        }

        tabs = FindObjectOfType<AbilityTabs>();
        SetSelectedUnit(defaultSelection);
        
    }

    float scrollDelay = 100;
    void Update()
    {
        scrollDelay += Time.deltaTime;
        if(Input.mouseScrollDelta.y != 0 && scrollDelay > .1f)
        {
            scrollDelay = 0;
            int dir = (int)Mathf.Sign(Input.mouseScrollDelta.y);
            selectedIndex += dir;

            if(selectedIndex >= units.Count)
            {
                selectedIndex = 0;
            } else if (selectedIndex < 0)
            {
                selectedIndex = units.Count - 1;
            }

            if (!units[selectedIndex].selectable)
            {
                selectedIndex += dir;
                if (selectedIndex >= units.Count)
                {
                    selectedIndex = 0;
                }
                else if (selectedIndex < 0)
                {
                    selectedIndex = units.Count - 1;
                }
            }

            SetSelectedUnit(units[selectedIndex]);
        }

        if (!selectedUnit.selectable)
            SetSelectedUnit(defaultSelection);

        if (Input.GetMouseButtonDown(0) && selectedAbilitySlot < selectedUnit.abilities.Count)
        {
            if(selectedUnit != null && Tower.instance.activeWave)
            {
                selectedUnit.abilities[selectedAbilitySlot].TryTriggerAbility(selectedUnit);
            }
            
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                
                if(hit.collider != null)
                {
                    PlayerUnit u = hit.collider.GetComponent<PlayerUnit>();
                    if(u != null)
                    {
                        SetSelectedUnit(u);
                    }
                }
            
                
        }
    
        
    }

    public void SetSelectedUnit(PlayerUnit p)
    {
        selectedUnit.OnDeselect();
        selectedIndex = p.unitIndex;
        p.OnSelect();
        selectedUnit = p;
        selectedAbilitySlot = 0;
        tabs.Initialize(p.abilities);
        FindObjectOfType<HPBar>().SetCharacter(p);
        FindObjectOfType<AttackModeSelector>().UpdateTargetType(p);
    }

    public bool ValidMapClick()
    {
        Vector2 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        if(p.x > minSelectableAreaBounds.x && p.x < maxSelectableAreaBounds.x && p.y > minSelectableAreaBounds.y && p.y < maxSelectableAreaBounds.y)
        {
            return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube((minSelectableAreaBounds + maxSelectableAreaBounds) / 2, new Vector3(maxSelectableAreaBounds.x - minSelectableAreaBounds.x, maxSelectableAreaBounds.y - minSelectableAreaBounds.y, 0));
    }
}
