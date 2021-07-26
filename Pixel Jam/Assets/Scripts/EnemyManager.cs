using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    #region Singleton
    public static EnemyManager instance;
    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    #endregion

    public List<Enemy> enemies = new List<Enemy>();
    
    public void AddEnemy(Enemy e)
    {
        enemies.Add(e);
    }

    public void RemoveEnemy(Enemy e)
    {
        enemies.Remove(e);
    }

    public Enemy GetNearestEnemy()
    {
        if(enemies.Count == 0)
        {
            return null;
        }

        Vector2 relativePoint = Tower.instance.climbingTop.position;

        Enemy nearest = null;
        float nearestDist = float.MaxValue;
        foreach(Enemy e in enemies)
        {
            float d = Vector2.Distance(relativePoint, e.transform.position);
            if (d < nearestDist)
            {
                nearestDist = d;
                nearest = e;
            }
        }

        return nearest;
    }
}
