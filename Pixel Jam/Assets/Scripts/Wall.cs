using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Wall : MonoBehaviour, IDamagable
{
    SpriteRenderer spriteRenderer;
    public float HP = 6;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = Mathf.RoundToInt(Mathf.Lerp(0, 5000, Mathf.InverseLerp(0, -6f, transform.position.y)));
    }

    public void ReceieveDamage(float damage)
    {


        damage /= 60;

        HP -= damage;
        Sequence s = DOTween.Sequence();
        s.Append(spriteRenderer.DOColor(Color.red, .1f));
        s.Append(spriteRenderer.DOColor(Color.white, .1f));

        if(HP <= 0)
        {
            Tower.instance.audioSource.PlayOneShot(Tower.instance.towerInteract);
            Destroy(gameObject);
        }
    }
}
