using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Giant : Enemy
{

    Vector2 targetPoint;
    GiantState state;

    public override void Start()
    {
        base.Start();
        targetPoint = Tower.instance.GetRandomPointAlongWall();
        attackTarget = Tower.instance;
    }


    public override void Update()
    {
        base.Update();

        switch (state)
        {
            case (GiantState.APPROACH):
                Approaching();
                break;

            case (GiantState.ATTACK):
                Attacking();
                break;
        }
    }

    void Approaching()
    {
        Move((targetPoint - transform.position.Flat()).normalized);

        if(Vector2.Distance(transform.position, targetPoint) < .05f)
        {
            state = GiantState.ATTACK;
            aniamtor.Play("Idle");
            velocity = Vector2.zero;
        }
    }

    void Attacking()
    {
        Attack();
    }
}

public enum GiantState
{
    APPROACH,
    ATTACK
}
