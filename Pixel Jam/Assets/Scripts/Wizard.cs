using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : PlayerUnit
{
    public GameObject missileProjectile;
    public void MagicMissile()
    {
        Vector2 p;

        if (Controller.instance.selectedUnit != this)
        {
            Enemy e = EnemyManager.instance.GetNearestEnemy();

            if (e == null)
                return;

            p = GetTargetPoint(40, e);
        }
        else
        {
            p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        GameObject proj = Instantiate(missileProjectile);
        proj.transform.position = projectileSpawnPoint.position;
        Projectile missile = proj.GetComponent<Projectile>();

        missile.InitializeProjectile(((p - projectileSpawnPoint.position.Flat()).normalized), p, 40, GetDamage());
    }
    public LayerMask corpseMask;
    public GameObject explosion;

    public void Necrophagy()
    {
        Vector2 p;

        if (Controller.instance.selectedUnit != this)
        {
            Enemy e = EnemyManager.instance.GetNearestEnemy();

            if (e == null)
                return;

            p = GetTargetPoint(40, e);
        }
        else
        {
            p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        Collider2D h = Physics2D.OverlapCircle(p, .25f, corpseMask);

        if (h != null)
        {
            Destroy(h.gameObject);
            Instantiate(explosion, p, Quaternion.identity);
        }
    }

    public void Barrier()
    {
        Vector2 p;

        if (Controller.instance.selectedUnit != this)
        {
            Enemy e = EnemyManager.instance.GetNearestEnemy();

            if (e == null)
                return;

            p = GetTargetPoint(40, e);
        }
        else
        {
            p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (p.x < maxWallPlacement.x && p.y < maxWallPlacement.y)
        {
            BoxCollider2D col = wallPrefab.GetComponent<BoxCollider2D>();

            bool validPlacement = true;

            Collider2D[] hits = Physics2D.OverlapBoxAll(p + col.offset, col.size*.65f, 0);
            foreach(Collider2D hit in hits)
            {
                Enemy e = hit.GetComponent<Enemy>();
                Wall overlap = hit.GetComponent<Wall>();

                if(e != null || overlap != null)
                {
                    validPlacement = false;
                }
            }

            if (validPlacement)
            {
                GameObject g = Instantiate(wallPrefab, p, Quaternion.identity);

                Wall w = g.GetComponent<Wall>();
                w.HP = GetDamage() * 15f;
            }

        }
    }

    public Vector2 maxWallPlacement;
    public GameObject wallPrefab;

    public override void Update()
    {
        base.Update();

        if(Controller.instance.selectedUnit != this && Tower.instance.activeWave)
        {
            abilities[autoSelectedAbility].TryTriggerAbility(this);
        }
    }
}
