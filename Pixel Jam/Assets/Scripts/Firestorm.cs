using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Firestorm : MonoBehaviour
{
    [SerializeField] float length = 7f;
    [SerializeField]SpriteRenderer bg;
    [SerializeField] ParticleSystem rain;
    [SerializeField] ParticleSystem smoke;


    public void TriggerFireStorm()
    {
        bg.DOFade(.75f, .3f);
        rain.Play();
        smoke.Play();

        Sequence effect = DOTween.Sequence();
        effect.AppendInterval(2.75f);
        effect.AppendCallback(FirestormEffect);

        Sequence s = DOTween.Sequence();
        s.AppendInterval(length);
        s.AppendCallback(EndFirestorm);
    }

    public void FirestormEffect()
    {
        foreach(Enemy e in EnemyManager.instance.enemies)
        {
            if(Random.value > .2f)
            {
                e.ApplyStatus(Status.BURN, 7);
            }
        }
    }

    public void EndFirestorm()
    {
        bg.DOFade(0, .3f);
        rain.Stop();
        smoke.Stop();
    }
}
