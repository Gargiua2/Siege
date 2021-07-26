using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    float damage;
    PlayerUnit target;
    public float speed = 5f;
    Vector2 heading;
    Vector2 startPos;
    float goalDist = 0;
    SpriteRenderer spriteRenderer;
    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void FireArrow(Vector2 position, PlayerUnit _target,float _damage)
    {
        transform.position = position;

        heading = (_target.transform.position.Flat() - position).normalized;
        transform.right = heading;

        target = _target;

        startPos = position;
        damage = _damage;

        goalDist = Vector2.Distance(position, _target.transform.position);
    }

    void Update()
    {

        spriteRenderer.sortingOrder = Mathf.RoundToInt(Mathf.Lerp(0, 5000, Mathf.InverseLerp(0, -6f, transform.position.y)));
        transform.position += (transform.right* speed * Time.deltaTime);

        if(Vector2.Distance(transform.position, startPos) > goalDist)
        {
            target.RecieveDamage(damage);
            Destroy(gameObject);
        }
    }
}
