using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float radius;
    Vector2 heading;
    float speed;
    float damage;
    int layer;
    Vector2 sourcePoint;
    float goalDist;
    Vector2 goal;

    Status status;
    float statusChance;
    float statusLength;
    Animator animator;

    bool spawner = false;
    GameObject toSpawn;
    float projectileLifetime;
    float despawnLength;
    SpriteRenderer spriteRenderer;

    public void InitializeProjectile(Vector2 _heading, Vector2 goalPoint, float _speed, float _damage, Status s = Status.NONE, float _statusLength = .5f, float _statusChance = 1)
    {
        goal = goalPoint;
        heading = _heading.normalized;
        speed = _speed;
        damage = _damage;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 5;
        goalDist = Vector2.Distance(goalPoint, transform.position);
        sourcePoint = transform.position;
        statusChance = _statusChance;
        status = s;
        statusLength = _statusLength;
        transform.right = -_heading;
        animator = GetComponent<Animator>();
    }
    public void InitializeProjectile(Vector2 _heading, Vector2 goalPoint, float _speed, GameObject prefab, float toDespawn, float lifetime)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 5;
        goal = goalPoint;
        heading = _heading.normalized;
        speed = _speed;
        damage = 0;
        goalDist = Vector2.Distance(goalPoint, transform.position);
        sourcePoint = transform.position;
        transform.right = -_heading;
        animator = GetComponent<Animator>();
        spawner = true;
        toSpawn = prefab;
        projectileLifetime = lifetime;
        despawnLength = toDespawn;
    }

    void Update()
    {
        spriteRenderer.sortingOrder = Mathf.RoundToInt(Mathf.Lerp(0, 5000, Mathf.InverseLerp(0, -6f, transform.position.y)));
        Vector2 nextPos = transform.position.Flat() + (heading * speed * Time.deltaTime);

        if (!spawner)
        {
            RaycastHit2D[] hits;
            hits = Physics2D.CircleCastAll(transform.position, radius * transform.localScale.x, heading, (heading * speed * Time.deltaTime).magnitude);

            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    Enemy e = hits[i].transform.GetComponent<Enemy>();
                    if (e != null)
                    {

                        if (status != Status.NONE && Random.value <= statusChance)
                        {
                            e.ApplyStatus(status, statusLength);
                        }
                        e.ReceiveDamage(damage);

                        if (animator != null)
                            animator.Play("Destroy");

                        transform.DetachChildren();

                        transform.position += (Vector3)((transform.position.Flat() - hits[i].point).magnitude * heading);
                        Destroy(gameObject, .15f);
                        Destroy(this);
                    }
                }
            }
        }

        transform.position = nextPos;

        if(Vector2.Distance(transform.position, sourcePoint) >= goalDist)
        {
            if (!spawner)
            {
                if (animator != null)
                    animator.Play("Destroy");
                transform.DetachChildren();
                Destroy(gameObject, .15f);
                Destroy(this);
            } else
            {
                transform.position = goal;
                if (animator != null)
                    animator.Play("Destroy");
                transform.DetachChildren();
                Destroy(gameObject, projectileLifetime);
                GameObject spawn = Instantiate(toSpawn, transform.position, Quaternion.identity);
                Destroy(spawn, despawnLength);
                Destroy(this);
            }
            
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, radius * transform.localScale.x);
    }
}
