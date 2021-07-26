using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Blizzard : MonoBehaviour
{
    [SerializeField] float length = 7f;
    [SerializeField] SpriteRenderer bg;
    [SerializeField] ParticleSystem rain;

    public void TriggerBlizzard()
    {
        bg.DOFade(.75f, .3f);
        rain.Play();

        Sequence effect = DOTween.Sequence();
        effect.AppendInterval(1.75f);
        effect.AppendCallback(BlizzardEffect);

        Sequence s = DOTween.Sequence();
        s.AppendInterval(length);
        s.AppendCallback(EndBlizzard);
    }

    public void BlizzardEffect()
    {
        foreach (Enemy e in EnemyManager.instance.enemies)
        {
            if (Random.value > .2f)
            {
                e.ApplyStatus(Status.FREEZE, 12);
            }
        }
    }

    public void EndBlizzard()
    {
        bg.DOFade(0, .3f);
        rain.Stop();
    }
}
