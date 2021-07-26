using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ShopAbilityUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Button buy;
    [SerializeField] TextMeshProUGUI cost;
    [SerializeField] TooltipTrigger tooltip;

    PlayerUnit u;
    int i;
    public void Initialize(PlayerUnit p, int index, Shop shop)
    {
        icon.sprite = p.lockedAbilities[index].icon;
        cost.text = "x " + p.lockedAbilities[index].cost;
        tooltip.header = p.lockedAbilities[index].name;
        tooltip.body = p.lockedAbilities[index].description;
        buy.onClick.AddListener(() => {shop.TryPurchase(p,index);});
    }
}
