using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : PlayerUnit
{
    public GameObject fireballProjectile;
    public GameObject fireOrbProjectile;
    public GameObject fireLine;
    public void Fireball()
    {
        Vector2 p;

        if (Controller.instance.selectedUnit != this)
        {
            Enemy e = EnemyManager.instance.GetNearestEnemy();

            if (e == null)
                return;

            p = GetTargetPoint(22, e);
        }
        else
        {
            p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        GameObject proj = Instantiate(fireballProjectile);
        proj.transform.position = projectileSpawnPoint.position;
        Projectile missile = proj.GetComponent<Projectile>();

        missile.InitializeProjectile(((p - projectileSpawnPoint.position.Flat()).normalized), p, 22, GetDamage());
    }

    public void Torch()
    {
        Vector2 p;

        if (Controller.instance.selectedUnit != this)
        {
            Enemy e = EnemyManager.instance.GetNearestEnemy();

            if (e == null)
                return;

            p = GetTargetPoint(50, e);
        }
        else
        {
            p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if(p.y > -.25f)
        {
            return;
        }

        GameObject proj = Instantiate(fireOrbProjectile);
        proj.transform.position = projectileSpawnPoint.position;
        Projectile missile = proj.GetComponent<Projectile>();

        missile.InitializeProjectile(((p - projectileSpawnPoint.position.Flat()).normalized), p, 50, fireLine, 3,3);
    }

    public override void Update()
    {
        base.Update();

        if (Controller.instance.selectedUnit != this && Tower.instance.activeWave)
        {
            abilities[autoSelectedAbility].TryTriggerAbility(this);
        }
    }
}
