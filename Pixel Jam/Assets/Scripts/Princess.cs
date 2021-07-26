using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Princess : PlayerUnit
{
    public bool kidnapped = false;
    public bool runningAway = false;
    public float speed;
    [HideInInspector] public bool captureable = true;
    [HideInInspector] public bool climbing = false;
    public Action OnKidnapped;
    public Action OnEscape;
    float escapeTimer = 0;
    public override void Update()
    {
        base.Update();

        if (!kidnapped && !runningAway && Controller.instance.selectedUnit != this && Tower.instance.activeWave)
            abilities[autoSelectedAbility].TryTriggerAbility(this);

        escapeTimer += Time.deltaTime;

            captureable = true;
        

        if (runningAway)
        {
            if (!climbing)
            {
                transform.Translate((Tower.instance.climbingBottom.transform.position.Flat() - transform.position.Flat()).normalized * speed * Time.deltaTime);

                if(Vector2.Distance(transform.position, Tower.instance.climbingBottom.transform.position) < .05f)
                {
                    climbing = true;
                }

            }else
            {
                transform.Translate((Tower.instance.climbingTop.transform.position.Flat() - transform.position.Flat()).normalized * speed * Time.deltaTime);

                if(Vector2.Distance(transform.position, Tower.instance.climbingTop.position) < .05f)
                {
                    runningAway = false;
                    selectable = true;
                    transform.SetParent(Tower.instance.towerTop);
                    GetComponent<SpriteRenderer>().flipX = false;
                }
            }
        }

        if(transform.position.x < -28 && Time.timeScale > 0)
        {
            FindObjectOfType<WaveTransition>().Transition("Game Over!", Tower.instance.GameOver);
        }
    }

    public GameObject heartProjectile;

    public void Heartbreak()
    {
        Vector2 p;

        if (Controller.instance.selectedUnit != this)
        {
            Enemy e = EnemyManager.instance.GetNearestEnemy();

            if (e == null)
                return;

            p = GetTargetPoint(15, e);
        }
        else
        {
            p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        GameObject proj = Instantiate(heartProjectile);
        proj.transform.position = projectileSpawnPoint.position;
        Projectile missile = proj.GetComponent<Projectile>();

        missile.InitializeProjectile(((p - projectileSpawnPoint.position.Flat()).normalized), p, 15, 0, Status.STUN, GetDamage(), 1);
    }

    public Transform SacrificeDamagePoint;
    public void Sacrifice()
    {
        if(Tower.instance.extraLayers > 0)
        {
            Tower.instance.Fall();
            Collider2D[] overlaps = Physics2D.OverlapCircleAll(SacrificeDamagePoint.position, 10f);

            foreach(Collider2D overlap in overlaps)
            {
                Enemy e = overlap.GetComponent<Enemy>();

                if (e != null)
                {
                    float dist = Vector2.Distance(SacrificeDamagePoint.position, e.transform.position);
                    e.ReceiveDamage(Mathf.Lerp(GetDamage()/2, GetDamage()*10, dist / 6f));
                }
            }
        }
    }

    public void Alchemy()
    {
        Tower.instance.AddCoins(UnityEngine.Random.Range(10, 20));
    }

    public void Kidnap()
    {
        kidnapped = true;
        runningAway = false;    
        selectable = false;
        OnKidnapped.Invoke();
    }

    public void Escape()
    {
        escapeTimer = 0;
        captureable = false;

        kidnapped = false;
        runningAway = true;
        GetComponent<SpriteRenderer>().flipX = true;
        OnEscape?.Invoke();
    }
}
