using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Knight : Enemy
{
    KnightState state;
    public float climbingSpeed;
    public Vector2 speedTweakRange = Vector2.one;
    public Transform princessHoldPostion;
    bool climbing = false;
    bool inLane = true;
    bool carryingPrincess = false;

    Vector2 selectedLaneEndPos;

    public override void Start()
    {
        base.Start();
        selectedLaneEndPos = new Vector2(Tower.instance.laneEnd.position.x, Mathf.Lerp(Tower.instance.lanes[UnityEngine.Random.Range(0, Tower.instance.lanes.Length)].transform.position.y - .2f, Tower.instance.lanes[UnityEngine.Random.Range(0, Tower.instance.lanes.Length)].transform.position.y + .2f, UnityEngine.Random.value));
        Tower.instance.princess.OnKidnapped += PrincessKidnapped;
        Tower.instance.princess.OnEscape += PrincessEscaped;
        Tower.instance.OnTowerDrop += OnTowerFall;


        speed = UnityEngine.Random.Range(speedTweakRange.x, speedTweakRange.y) * speed;

        if (Tower.instance.princess.kidnapped)
        {
            state = KnightState.Retreating;
        } else if (Tower.instance.princess.runningAway)
        {
            state = KnightState.Chasing;
        }
        else
        {
            state = KnightState.Seiging;
        }
    } 

    public void OnTowerFall()
    {
        if(climbing && transform.position.y > -1.75)
        {
            transform.position -= Vector3.up;
        }
    }

    public void PrincessKidnapped()
    {
        
        state = KnightState.Retreating;
    }

    public void PrincessEscaped()
    {
        state = KnightState.Chasing;    
    }

    public override void Update()
    {
        base.Update();

        switch (state)
        {
            case(KnightState.Seiging):
                aniamtor.Play("Walk");
                Seige();
                break;

            case (KnightState.Retreating):
                
                Retreat();
                break;

            case (KnightState.Chasing):
                Chase();
                break;
        }

    }

    void Seige()
    {
        if (!climbing)
        {
            if (transform.position.x < Tower.instance.laneEnd.position.x)
            {
                Move(Vector3.right);
            }
            else
            {
                if (Vector2.Distance(transform.position, Tower.instance.climbingBottom.position) < .05f)
                {
                    climbing = Tower.instance.TryStartClimb();
                    if (climbing)
                    {
                        aniamtor.Play("Climb");
                    }
                } else
                {
                    Move(((Vector2)Tower.instance.climbingBottom.position - (Vector2)transform.position).normalized);
                    layer = 0;
                    inLane = false;
                }
                
            }
        }

        if (climbing)
        {
            Move(((Vector2)Tower.instance.climbingTop.position - (Vector2)transform.position).normalized * climbingSpeed);
            
            if(Vector2.Distance(Tower.instance.climbingTop.position, transform.position) < .01f)
            {
                Tower.instance.princess.Kidnap();
                carryingPrincess = true;
                Tower.instance.princess.transform.position = princessHoldPostion.position;
                Tower.instance.princess.transform.SetParent(transform);
            }
        }

       

    }

    void Retreat()
    {
        if (climbing)
        {
            Move((Tower.instance.climbingBottom.position.Flat() - transform.position.Flat()).normalized * climbingSpeed);

            if(Vector2.Distance(transform.position, Tower.instance.climbingBottom.position) < .01f)
            {
                climbing = false;
                aniamtor.Play("Walk");
            }
        }

        if (!climbing)
        {
            if (!inLane)
            {
                Move((selectedLaneEndPos - transform.position.Flat()).normalized);
                if(Vector2.Distance(transform.position, selectedLaneEndPos) < .1f)
                {
                    inLane = true;
                }
            } else
            {
                Move(Vector2.left);
            }
        }
    }

    void Chase()
    {
        bool princessAbove = Tower.instance.princess.transform.position.y > transform.position.y;
        bool princessRight = Tower.instance.princess.transform.position.x > transform.position.x;

        if (climbing)
        {
            if (princessAbove)
            {
                Move(((Vector2)Tower.instance.climbingTop.position - (Vector2)transform.position).normalized * climbingSpeed);
            }
            else
            {
                Move((Tower.instance.climbingBottom.position.Flat() - transform.position.Flat()).normalized * climbingSpeed);

                if(Vector2.Distance(Tower.instance.climbingBottom.position, transform.position) < .05f)
                {
                    climbing = false;
                    aniamtor.Play("Walk");
                }
            }
        } else if (inLane)
        {
            if (princessRight)
            {
                Move(Vector3.right);

                if(transform.position.x > Tower.instance.laneEnd.position.x)
                {
                    inLane = false;
                }

            } else
            {
                Move(-Vector3.right);
            }
        }

        if (!climbing && !inLane)
        {
            if (princessRight || princessAbove)
            {
                Move(((Vector2)Tower.instance.climbingBottom.position - (Vector2)transform.position).normalized);
                if(Vector2.Distance(transform.position, Tower.instance.climbingBottom.position)<.05f)
                {
                    climbing = Tower.instance.TryStartClimb();
                    if(climbing)
                    aniamtor.Play("Climb");
                }
            }
            else
            {
                Move((selectedLaneEndPos - transform.position.Flat()).normalized);
                if (Vector2.Distance(transform.position, selectedLaneEndPos) < .1f)
                {
                    inLane = true;
                }
            }
        }

        if (Vector2.Distance(Tower.instance.princess.transform.position, transform.position) < 2f && Tower.instance.princess.captureable)
        {
            Tower.instance.princess.Kidnap();
            carryingPrincess = true;
            Tower.instance.princess.transform.position = princessHoldPostion.position;
            Tower.instance.princess.transform.SetParent(transform);
        }
    }

    public override void Die()
    {
        if (carryingPrincess)
        {
            Tower.instance.princess.transform.position = transform.position;
            Tower.instance.princess.transform.SetParent(null);
            Tower.instance.princess.Escape();
            Tower.instance.princess.climbing = climbing;
        }

        base.Die();
    }

    private void OnDestroy()
    {
        Tower.instance.princess.OnKidnapped -= PrincessKidnapped;
        Tower.instance.princess.OnEscape -= PrincessEscaped;
        Tower.instance.OnTowerDrop -= OnTowerFall;
    }

}


public enum KnightState
{
    Seiging,
    Retreating,
    Chasing
}