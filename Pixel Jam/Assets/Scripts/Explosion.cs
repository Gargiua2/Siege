using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float range = 4.5f;
    void Start()
    {
        GetComponent<ParticleSystemRenderer>().sortingOrder = Mathf.RoundToInt(Mathf.Lerp(0, 5000, Mathf.InverseLerp(0, -6f, transform.position.y)))+5;
        Wizard w = FindObjectOfType<Wizard>();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);
        foreach (Collider2D hit in hits)
        {
            Enemy e = hit.GetComponent<Enemy>();

            if (e != null)
            {
                e.ReceiveDamage(w.GetDamage() * 3.2f);
            }
        }

        Destroy(gameObject, 1);
    }


}
