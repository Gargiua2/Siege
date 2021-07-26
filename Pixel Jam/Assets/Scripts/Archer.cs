using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Archer : Enemy
{
    ArcherState state;
    public Transform projectileFirePoint;
    public GameObject arrow;
    Vector2 attackPoint;
    public override void Start()
    {
        base.Start();
        attackTarget = Tower.instance;
        state = ArcherState.APPROACH;

        attackPoint = Tower.instance.GetRandomArcherPoint();
    }


    public override void Update()
    {
        base.Update();

        switch (state)
        {
            case (ArcherState.APPROACH):
                Approaching();
                break;

            case (ArcherState.ATTACK):
                Attacking();
                break;
        }
    }

    public void Approaching()
    {
        Move((attackPoint - transform.position.Flat()).normalized);
        
        if(Vector2.Distance(transform.position, attackPoint) < .05f)
        {
            state = ArcherState.ATTACK;
            aniamtor.Play("Idle");
        }
    }

    public void Attacking()
    {
        Attack();
    }

    public override void Attack()
    {
        if (attackCooldown > attackDelay)
        {
            Sequence s = DOTween.Sequence();
            s.AppendInterval(attackAnimationImpactDelay);
            s.AppendCallback(FireArrow);

            if (aniamtor != null)
            {
                aniamtor.Play("Attack");
            }
            attackCooldown = 0;
        }
    }

    public void FireArrow()
    {
        PlayerUnit target = Tower.instance.attackableUnits[Random.Range(0, Tower.instance.attackableUnits.Count)];
        GameObject g = Instantiate(arrow); 
        g.GetComponent<Arrow>().FireArrow(transform.position.Flat(), target, damage);
    }
}

public enum ArcherState
{
    APPROACH,
    ATTACK
}