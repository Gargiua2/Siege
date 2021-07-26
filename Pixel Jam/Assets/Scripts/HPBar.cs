using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HPBar : MonoBehaviour
{
    public ProgressBar HP;
    public Image i;
    PlayerUnit p;

    public void SetCharacter(PlayerUnit p)
    {
        i.sprite = p.portrait;
    }
}
