using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackModeSelector : MonoBehaviour
{
    public Button enemyMode;
    public Button targetMode;
    public Image enemySelected;
    public Image targetSelected;

    void Start()
    {
        enemyMode.onClick.AddListener(() => SwapTargetType(true));
        targetMode.onClick.AddListener(() => SwapTargetType(false));
        SwapTargetType(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            SwapTargetType(!currentMode);
        }    
    }

    public void UpdateTargetType(PlayerUnit p)
    {
        if (p.targetMode)
        {
            currentMode = false;
            enemySelected.enabled = true;
            targetSelected.enabled = false;
        }
        else
        {
            currentMode = true;
            enemySelected.enabled = false;
            targetSelected.enabled = true;
        }
    }
    bool currentMode = true;
    public void SwapTargetType(bool mode)
    {
        currentMode = mode;
        if (mode)
        {
            enemySelected.enabled = true;
            targetSelected.enabled = false;
        } else
        {
            enemySelected.enabled = false;
            targetSelected.enabled = true;
        }

        Controller.instance.selectedUnit.SwapTargetMode(mode);
    }
}
