using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    public float maxHP;
    float currentHP;
    public float damage = 2;
    public float attackDelay = .5f;
    public float attackAnimationImpactDelay = .2f;
    public Color damageFlashColor = Color.red;
    SpriteRenderer spriteRenderer;
    public float speed;
    public AnimationCurve coinOdds;
    public Vector2Int coinRange;
    public int inverted = 1;
    [HideInInspector] public int layer;
    public Animator aniamtor;
    public IDamagable attackTarget;


    [HideInInspector]public float attackCooldown = 100;

    public virtual void Start()
    {
        currentHP = maxHP;
        EnemyManager.instance.AddEnemy(this);
        spriteRenderer = GetComponent<SpriteRenderer>();
        InvokeRepeating("Tick", 0, .25f);
        aniamtor = GetComponent<Animator>();
    }
    public Vector3 velocity;
    [HideInInspector]public bool overrideMoveAnimation = false;
    public virtual void Move(Vector2 direction)
    {
        float s = speed;

        if (frozen)
            s /= 2;

        if (stunned)
            s = 0;

        if(Mathf.Sign(direction.x * inverted) >= 0)
        {
            spriteRenderer.flipX = true;
        } else
        {
            spriteRenderer.flipX = false;
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, Time.deltaTime * s, Tower.instance.wall);

        if(hit.collider == null)
        {
            transform.Translate(direction * Time.deltaTime * s);
            velocity = direction * Time.deltaTime * s;
        }
        else
        {
            hit.collider.GetComponent<Wall>().ReceieveDamage(damage);
            velocity = Vector3.zero;
        }
        
    }

    public virtual void Update()
    {
        UpdateStatuses();
        spriteRenderer.sortingOrder = Mathf.RoundToInt(Mathf.Lerp(0, 5000, Mathf.InverseLerp(0, -6f, transform.position.y)));
        attackCooldown += Time.deltaTime;
    }

    public void Tick()
    {
        if (burned)
            ReceiveDamage(.75f);
    }

    public virtual void Attack()
    {
        if(attackCooldown > attackDelay)
        {
            Sequence s = DOTween.Sequence();
            s.AppendInterval(attackAnimationImpactDelay);
            s.AppendCallback(() => attackTarget.ReceieveDamage(damage));

            if(aniamtor != null)
            {
                aniamtor.Play("Attack");
            }
            attackCooldown = 0;
        }
    }

    Sequence damageFlash;
    public void ReceiveDamage(float damage)
    {
        if(damage > 2f)
        {
            Tower.instance.PlayDamageSFX();
        }

        currentHP -= damage;

            damageFlash = DOTween.Sequence();
            damageFlash.Append(spriteRenderer.DOColor(damageFlashColor, .15f)).SetEase(Ease.Flash);
            damageFlash.AppendInterval(.1f);
            damageFlash.Append(spriteRenderer.DOColor(Color.white, .1f));
        


        if (currentHP <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        aniamtor.enabled = false;
        float roll = Mathf.Clamp01(coinOdds.Evaluate(Random.Range(0, 1f)));

        int amount = Mathf.RoundToInt(Mathf.Lerp(coinRange.x, coinRange.y,roll));

        Tower.instance.AddCoins(amount);

        EnemyManager.instance.RemoveEnemy(this);

        Sequence s = DOTween.Sequence();
        s.Append(transform.DORotate(new Vector3(0, 0, 90), .3f));
        s.Append(spriteRenderer.DOColor(new Color(damageFlashColor.r/2, damageFlashColor.g / 2, damageFlashColor.b / 2,1), .15f)).SetEase(Ease.Flash);
        s.AppendInterval(7.55f);
        s.Append(spriteRenderer.DOFade(0, 2f));

        gameObject.layer = 7;

        Destroy(gameObject, 10f);

        Destroy(this);
        
    }

    bool stunned = false;
    float stunTimer = 0;
    float stunLength = 0;

    bool burned = false;
    float burnTimer = 0;
    float burnLength = 0;

    bool frozen = false;
    float freezeTimer = 0;
    float freezeLength = 0;

    public void UpdateStatuses()
    {
        stunTimer += Time.deltaTime;
        burnTimer += Time.deltaTime;
        freezeTimer += Time.deltaTime;

        if (freezeTimer > freezeLength && frozen)
        {
            Destroy(freezeFX);
            frozen = false;
        }
            

        if (burnTimer > burnLength && burned)
        {
            Destroy(burnFX);
            burned = false;
        }
            

        if (stunTimer > stunLength && stunned)
        {
            Destroy(stunFX);
            stunned = false;
        }
            

    }

    GameObject stunFX;
    GameObject freezeFX;
    GameObject burnFX;
    public void ApplyStatus(Status s, float length)
    {

        switch (s)
        {
            case (Status.STUN):
                stunned = true;
                stunTimer = 0;
                stunLength = length;
                stunFX = Instantiate(Tower.instance.stunParticles);
                stunFX.transform.SetParent(transform);
                stunFX.transform.position = transform.position + Vector3.up * .45f;
                break;

            case (Status.BURN):
                burned = true;
                burnTimer = 0;
                burnLength = length;
                burnFX = Instantiate(Tower.instance.fireParticles);
                burnFX.transform.SetParent(transform);
                burnFX.transform.position = transform.position + Vector3.up * .45f;
                break;

            case (Status.FREEZE):
                frozen = true;
                freezeTimer = 0;
                freezeLength = length;
                freezeFX = Instantiate(Tower.instance.forzenParticles);
                freezeFX.transform.SetParent(transform);
                freezeFX.transform.position = transform.position + Vector3.up * .45f;
                break;
        }
    }

    
}

[System.Serializable]
public enum Status : int
{
    NONE,
    STUN,
    BURN,
    FREEZE
}