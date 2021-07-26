using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour
{
    SpriteRenderer spriteRender;
    Dragon dragon;
    void Start()
    {
        spriteRender = GetComponent<SpriteRenderer>();
        dragon = FindObjectOfType<Dragon>();
        spriteRender.sortingOrder = Mathf.RoundToInt(Mathf.Lerp(0, 5000, Mathf.InverseLerp(0, -6f, transform.position.y)))-1;

        InvokeRepeating("Tick", 0, .125f);
    }

    void Tick()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(6,1),0);

        foreach(Collider2D hit in hits)
        {
            Enemy e = hit.gameObject.GetComponent<Enemy>();

            if (e != null)
            {
                e.ReceiveDamage((dragon.GetDamage() * .65f) / 12f);

                if(Random.value < .1f)
                {
                    e.ApplyStatus(Status.BURN, .66f);
                }
            }
        }
    }

}
