using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Shop : MonoBehaviour
{
    [Header("Character Select")]
    [SerializeField] PlayerUnit Mage;
    [SerializeField] Button mageButton;
    [SerializeField] Image mageSelection;
    [SerializeField] PlayerUnit Princess;
    [SerializeField] Button princessButton;
    [SerializeField] Image princessSelection;
    [SerializeField] PlayerUnit Dragon;
    [SerializeField] Button dragonButton;
    [SerializeField] Image dragonSelection;

    [Header("Ability Selection")]
    [SerializeField] GameObject abilityPrefab;
    [SerializeField] Transform shopGrid;

    [Header("Stats")]
    [SerializeField] public Button attackStatButton;
    [SerializeField] public TextMeshProUGUI attackStatCost;
    [SerializeField] public TextMeshProUGUI attackLevelText;


    [SerializeField] public Button staminaStatButton;
    [SerializeField] public TextMeshProUGUI staminaStatCost;
    [SerializeField] public TextMeshProUGUI staminaLevelText;

    [SerializeField] public Button speedStatButton;
    [SerializeField] public TextMeshProUGUI speedStatCost;
    [SerializeField] public TextMeshProUGUI speedLevelText;

    [Header("Other")]
    [SerializeField] Button towerBuild;
    [SerializeField] TextMeshProUGUI towerBuildCost;
    [SerializeField] Button startWave;
    [SerializeField] TextMeshProUGUI coinCount;

    int towerBuildPrice = 0;

    public void OpenShop()
    {
        gameObject.SetActive(true);

        SelectCharacter(0);

        mageButton.onClick.RemoveAllListeners();
        dragonButton.onClick.RemoveAllListeners();
        princessButton.onClick.RemoveAllListeners();
        towerBuild.onClick.RemoveAllListeners();
        startWave.onClick.RemoveAllListeners();
        attackStatButton.onClick.RemoveAllListeners();
        staminaStatButton.onClick.RemoveAllListeners();
        speedStatButton.onClick.RemoveAllListeners();

        attackStatButton.onClick.AddListener(IncrementAttack);
        staminaStatButton.onClick.AddListener(IncrementStamina);
        speedStatButton.onClick.AddListener(IncrementSpeed);
        towerBuild.onClick.AddListener(TryBuildTower);
        mageButton.onClick.AddListener(() => {SelectCharacter(0);});
        dragonButton.onClick.AddListener(() => { SelectCharacter(1); });
        princessButton.onClick.AddListener(() => { SelectCharacter(2); });
        startWave.onClick.AddListener(FindObjectOfType<WaveManager>().StartWave);

        towerBuildPrice = towerPricePerLayer * (Tower.instance.extraLayers + 1);
        towerBuildCost.text = "x " + towerBuildPrice;
        if(Tower.instance.extraLayers+1 > Tower.instance.maxExtraLayers)
        {
            towerBuildCost.text = "x MAX";
        }
    }

    int currentPick = 0;
    PlayerUnit viewedCharacter;

    public void SelectCharacter(int pick)
    {
        currentPick = pick;

        if(pick == 0)
        {
            mageSelection.enabled = true;
            princessSelection.enabled = false;
            dragonSelection.enabled = false;
            InitializePurchaseableAbilities(Mage);
            viewedCharacter = Mage;
        }
        else if (pick == 1)
        {
            mageSelection.enabled = false;
            princessSelection.enabled = false;
            dragonSelection.enabled = true;
            InitializePurchaseableAbilities(Dragon);
            viewedCharacter = Dragon;
        }
        else if (pick == 2)
        {
            mageSelection.enabled = false;
            princessSelection.enabled = true;
            dragonSelection.enabled = false;
            InitializePurchaseableAbilities(Princess);
            viewedCharacter = Princess;
        }

        attackLevelText.text = viewedCharacter.damageLevel + "/5";
        staminaLevelText.text = viewedCharacter.staminaLevel + "/5";
        speedLevelText.text = viewedCharacter.attackSpeedLevel + "/5";

        if(viewedCharacter.damageLevel < 5)
        {
            attackStatCost.text = "x" + Tower.instance.attackCosts[viewedCharacter.damageLevel];
        } else
        {
            attackStatCost.text = "x MAX";
        }
        
        if(viewedCharacter.staminaLevel < 5)
        {
            staminaStatCost.text = "x" + Tower.instance.staminaCosts[viewedCharacter.staminaLevel];
        } else
        {
            staminaStatCost.text = "x MAX";
        }

        if (viewedCharacter.attackSpeedLevel < 5)
        {
            speedStatCost.text = "x" + Tower.instance.speedCosts[viewedCharacter.attackSpeedLevel];
        } else
        {
            speedStatCost.text = "x MAX";
        }
        
    }

    void Update()
    {
        coinCount.text = "x"+Tower.instance.coinCount;    
    }

    public void InitializePurchaseableAbilities(PlayerUnit p)
    {
        for(int i = shopGrid.childCount-1 ; i >= 0; i--)
        {
            DestroyImmediate(shopGrid.GetChild(i).gameObject);
        }

        for(int i = 0; i < p.lockedAbilities.Count; i++)
        {
            GameObject abilityItem = Instantiate(abilityPrefab);
            abilityItem.transform.SetParent(shopGrid, false);
            ShopAbilityUI slot = abilityItem.GetComponent<ShopAbilityUI>();
            slot.Initialize(p, i, this);
        }
        
    }

    public void IncrementAttack()
    {
        if(viewedCharacter.damageLevel < 5)
        {
            if(Tower.instance.coinCount >= Tower.instance.attackCosts[viewedCharacter.damageLevel])
            {
                Tower.instance.AddCoins(-Tower.instance.attackCosts[viewedCharacter.damageLevel]);
                viewedCharacter.damageLevel++;
                SelectCharacter(currentPick);
            }
        }
    }

    public void IncrementSpeed()
    {
        if (viewedCharacter.attackSpeedLevel < 5)
        {
            if (Tower.instance.coinCount >= Tower.instance.speedCosts[viewedCharacter.attackSpeedLevel])
            {
                Tower.instance.AddCoins(-Tower.instance.speedCosts[viewedCharacter.attackSpeedLevel]);
                viewedCharacter.attackSpeedLevel++;
                SelectCharacter(currentPick);
            }
        }
    }

    public void IncrementStamina()
    {
        if (viewedCharacter.staminaLevel < 5)
        {
            if (Tower.instance.coinCount >= Tower.instance.staminaCosts[viewedCharacter.staminaLevel])
            {
                Tower.instance.AddCoins(-Tower.instance.staminaCosts[viewedCharacter.staminaLevel]);
                viewedCharacter.staminaLevel++;
                SelectCharacter(currentPick);
            }
        }
    }

    public void TryPurchase(PlayerUnit p, int abilityIndex)
    {
        if(Tower.instance.coinCount >= p.lockedAbilities[abilityIndex].cost)
        {
            Tower.instance.AddCoins(-p.lockedAbilities[abilityIndex].cost);
            p.abilities.Add(p.lockedAbilities[abilityIndex]);
            p.lockedAbilities.RemoveAt(abilityIndex);

            if (Controller.instance.selectedUnit == viewedCharacter)
                FindObjectOfType<AbilityTabs>().Initialize(p.abilities);

            SelectCharacter(currentPick);
        }
    }
    public int towerPricePerLayer;
    public void TryBuildTower()
    {
        if (Tower.instance.extraLayers + 1 <= Tower.instance.maxExtraLayers && Tower.instance.coinCount >= towerBuildPrice)
        {
            Tower.instance.Grow();
            Tower.instance.AddCoins(-towerBuildPrice);
        }

        towerBuildPrice = towerPricePerLayer * (Tower.instance.extraLayers + 1);
        towerBuildCost.text = "x " + towerBuildPrice;
        if (Tower.instance.extraLayers + 1 > Tower.instance.maxExtraLayers)
        {
            towerBuildCost.text = "x MAX";
        }
    }
}
