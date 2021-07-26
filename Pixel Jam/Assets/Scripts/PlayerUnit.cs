using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class PlayerUnit : MonoBehaviour
{
    [HideInInspector]public int unitIndex = 0;
    //VARIABLES------------------------------------------------------------------------------------------------------------//
    [HideInInspector] public float stunDamage;

    //Stun duation
    public float fullStunDuration;
    [HideInInspector]public float stunDuration;
    public bool stunned = false;

    public Transform projectileSpawnPoint;
    public Vector2 damageRange;
    [HideInInspector] public int damageLevel = 0;

    public Vector2 staminaRange;
    [HideInInspector] public int staminaLevel = 0;

    public Vector2 attackSpeedRange;
    [HideInInspector] public int attackSpeedLevel = 0;



    //All abilities this unit has access to.
    public List<Ability> abilities;
    public List<Ability> lockedAbilities;

    //UI Stuff
    public Sprite portrait;
    public Sprite stunnedPortrait;
    public GameObject target;

    [HideInInspector] public LaneSpawner[] lanes;
    [HideInInspector] public bool selectable = true;
    HPBar bar;
    SpriteRenderer spriteRenderer;
    [HideInInspector] public Animator animator;
    [HideInInspector]public bool targetMode = true;
    public Sprite stunnedSprite;
    //---------------------------------------------------------------------------------------------------------------------//
    //CODE------------------------------------------------------------------------------------------------------------------//
    //--------------------------------------------------------------------------------------------------------------------//

    public virtual void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        lanes = FindObjectsOfType<LaneSpawner>();
        bar = FindObjectOfType<HPBar>();
        stunDamage = 0;
    }

    public void Refresh()
    {
        foreach(Ability a in abilities)
        {
            a.RefreshAbility();
        }

        stunDamage = 0;

        stunDuration = fullStunDuration;
    }
    public float GetDamage()
    {
        return (Mathf.Lerp(damageRange.x, damageRange.y, damageLevel / 5));
    }

    public float GetStamina()
    {
        return (Mathf.Lerp(staminaRange.x, staminaRange.y, staminaLevel/ 5));
    }

    public float GetAttackSpeed()
    {
        return (Mathf.Lerp(attackSpeedRange.x, attackSpeedRange.y, attackSpeedLevel / 5));
    }

    public Vector2 GetTargetPoint(float shotSpeed, Enemy e)
    {
        if (targetMode)
        {
            return FirstOrderIntercept(projectileSpawnPoint.position, Vector3.zero, shotSpeed, e.transform.position, e.velocity);
        } else
        {
            return target.transform.position;
        }
    }

    [HideInInspector] public int autoSelectedAbility = 0;
    public void OnSelect()
    {
        

        if (!targetMode)
        {
            target.SetActive(true);
        } else
        {
            target.SetActive(false);
        }
    }

    public void OnDeselect()
    {
        autoSelectedAbility = Controller.instance.selectedAbilitySlot;
        target.SetActive(false);
    }

    public void SwapTargetMode(bool mode)
    {
        targetMode = mode;
        if (mode)
        {
            target.SetActive(false);
        } else
        {
            target.SetActive(true);
        }
    }

    //Check to see if the player is still stunned. Reset if not.
    //Update abilties.
    public virtual void Update()
    {
        stunDuration += Time.deltaTime;
        if (stunDuration > fullStunDuration)
        {
            
            if (Controller.instance.selectedUnit == this)
                bar.i.sprite = portrait;

            stunned = false;
        } else if (stunned && Controller.instance.selectedUnit == this)
        {
            bar.i.sprite = stunnedPortrait;
        }

        if (stunned)
        {
            spriteRenderer.sprite = stunnedSprite;
        }

        if(Controller.instance.selectedUnit == this)
        {
            if (stunned)
            {
                bar.HP.SetProgress(stunDuration / fullStunDuration);
            }
            else
            {
                bar.HP.SetProgress((GetStamina() - stunDamage) / GetStamina());
            }
        }

        if (!targetMode && Input.GetMouseButtonDown(1) && Controller.instance.selectedUnit == this && Tower.instance.activeWave)
        {
            Vector2 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.transform.position = new Vector3(mPos.x, mPos.y, 0); 
        }
        

        UpdateAbilties();
    }

    public LaneSpawner GetNearestLane(float pY)
    {
        LaneSpawner nearest = null;
        float nearestDist = float.MaxValue;
        foreach (LaneSpawner n in lanes)
        {
            if (Mathf.Abs(pY - n.transform.position.y) < nearestDist)
            {
                nearestDist = Mathf.Abs(pY - n.transform.position.y);
                nearest = n;
            }
        }

        return nearest;
    }

    //Called when we take damage, handles activating stun state. 
    public void RecieveDamage(float damage)
    {
        if (stunned)
            return;

        stunDamage += damage;

        Sequence s = DOTween.Sequence();
        s.Append(spriteRenderer.DOColor(Color.red, .1f));
        s.Append(spriteRenderer.DOColor(Color.white, .05f));

        if(stunDamage > GetStamina())
        {
            if(Controller.instance.selectedUnit == this)
                bar.i.sprite = stunnedPortrait;
            stunDuration = 0;
            stunDamage = 0;
            stunned = true;
        }
    }

    //FROM UNITY3D WIKI!
    //first-order intercept using absolute target position
    public Vector3 FirstOrderIntercept
    (
        Vector3 shooterPosition,
        Vector3 shooterVelocity,
        float shotSpeed,
        Vector3 targetPosition,
        Vector3 targetVelocity
    )
    {
        Vector3 targetRelativePosition = targetPosition - shooterPosition;
        Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
        float t = FirstOrderInterceptTime
        (
            shotSpeed,
            targetRelativePosition,
            targetRelativeVelocity
        );
        return targetPosition + t * (targetRelativeVelocity);
    }
    //first-order intercept using relative target position
    public float FirstOrderInterceptTime
    (
        float shotSpeed,
        Vector3 targetRelativePosition,
        Vector3 targetRelativeVelocity
    )
    {
        float velocitySquared = targetRelativeVelocity.sqrMagnitude;
        if (velocitySquared < 0.001f)
            return 0f;

        float a = velocitySquared - shotSpeed * shotSpeed;

        //handle similar velocities
        if (Mathf.Abs(a) < 0.001f)
        {
            float t = -targetRelativePosition.sqrMagnitude /
            (
                2f * Vector3.Dot
                (
                    targetRelativeVelocity,
                    targetRelativePosition
                )
            );
            return Mathf.Max(t, 0f); //don't shoot back in time
        }

        float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
        float c = targetRelativePosition.sqrMagnitude;
        float determinant = b * b - 4f * a * c;

        if (determinant > 0f)
        { //determinant > 0; two intercept paths (most common)
            float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a),
                    t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);
            if (t1 > 0f)
            {
                if (t2 > 0f)
                    return Mathf.Min(t1, t2); //both are positive
                else
                    return t1; //only t1 is positive
            }
            else
                return Mathf.Max(t2, 0f); //don't shoot back in time
        }
        else if (determinant < 0f) //determinant < 0; no intercept path
            return 0f;
        else //determinant = 0; one intercept path, pretty much never happens
            return Mathf.Max(-b / (2f * a), 0f); //don't shoot back in time
    }
    //

    //Calls the update Method on each of our abilties.
    public void UpdateAbilties()
    {
        for(int i = 0; i < abilities.Count; i++)
        {
            abilities[i].UpdateAbilty(this);
        }
    }

    public void TestAbility()
    {
        Debug.Log("You just triggered a test ability on the unit " + gameObject.name + "!");
    }

    //------------------------------------------------------------------------------------------------------------------//
    //END OF CLASS------------------------------------------------------------------------------------------------------//
    //------------------------------------------------------------------------------------------------------------------//
}

[System.Serializable]
public class Ability
{
    public string name;
    public string description;
    public Sprite icon;
    public float chargeTime;
    public UnityEvent toTrigger;
    public int cost;
    public AudioClip sfx;
    float chargeTimer;
    [HideInInspector] public AbilitySlotUI ui;
    public void RefreshAbility()
    {
        chargeTimer = chargeTime;

        if (ui != null)
        {
            ui.UpdateCharge(chargeTimer / chargeTime);
        }
    }

    public void UpdateAbilty(PlayerUnit p)
    {

        if (!p.stunned)
        {
            chargeTimer += Time.deltaTime * p.GetAttackSpeed();

            if(ui != null)
            {
                ui.UpdateCharge(chargeTimer / chargeTime);
            }
        }
    }

    public bool TryTriggerAbility(PlayerUnit p)
    {
        if(!p.stunned && chargeTimer > chargeTime)
        {
            if(sfx!=null)
                Tower.instance.audioSource.PlayOneShot(sfx);

            p.animator.Play("Attack");
            toTrigger?.Invoke();
            chargeTimer = 0;
            return true;
        }

        return false;
    }
}