using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LaneSpawner : MonoBehaviour
{
    public List<Subwave> waves; 

    public void TrySpawn(float t)
    {
        UpdateSubwaves(t);
    }

    void UpdateSubwaves(float t)
    {
        for(int i = 0; i < waves.Count; i++)
        {
            if (!waves[i].invoked && t > waves[i].time)
            {
                InvokeSubwave(waves[i]);
                waves[i] = new Subwave(waves[i], true);
                StartCoroutine(MarkSubwaveComplete(i, waves[i].trickleLength));
            }
        }
    }

    public void InvokeSubwave(Subwave s)
    {
        foreach (EnemyGroup g in s.groups)
        {
            int amount = Random.Range(g.amount.x, g.amount.y);
            for (int i = 0; i < amount; i++)
            {
                StartCoroutine(DelayUnitSpawn(g.enemy, Random.Range(0, s.trickleLength)));
            }
        }
    }

    public IEnumerator MarkSubwaveComplete(int index, float trickleTime)
    {
        yield return new WaitForSeconds(trickleTime);
        waves[index] = new Subwave(true);
    }

    public IEnumerator DelayUnitSpawn(GameObject prefab, float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnUnit(prefab);
    }

    public void SpawnUnit(GameObject unitPrefab)
    {
        GameObject unit = Instantiate(unitPrefab);
        unit.transform.position = transform.position + Vector3.Lerp(-Vector2.up * .2f, Vector2.up * .2f, Random.value);
        unit.transform.SetParent(transform);
    }

    public bool GetLaneCompletion()
    {
        bool completed = true;
        foreach(Subwave s in waves)
        {
            if (s.completed == false)
                completed = false;
        }

        return completed;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * 30);
    }
}

[System.Serializable]
public struct Subwave
{
    public float time;
    public float trickleLength;
    public List<EnemyGroup> groups;
    public bool invoked;
    public bool completed;

    public Subwave(bool c)
    {
        time = 0;
        trickleLength = 0;
        groups = new List<EnemyGroup>();
        invoked = true;
        completed = true;
    }

    public Subwave(Subwave s, bool _invoked)
    {
        time = s.time;
        trickleLength = s.trickleLength;
        groups = s.groups;
        invoked = _invoked;
        completed = false;
    }

    public Subwave(Subwave s)
    {

        time = s.time;
        trickleLength = s.trickleLength;
        groups = s.groups;
        invoked = true;
        completed = true;
    }
}

[System.Serializable]
public struct EnemyGroup
{
    public GameObject enemy;
    public Vector2Int amount;
}