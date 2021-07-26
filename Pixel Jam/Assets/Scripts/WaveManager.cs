using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class WaveManager : MonoBehaviour
{
    public int currentWave = 0;
    public List<Wave> waves;
    LaneSpawner[] lanes;
    float waveTime = 0;
    public TextMeshProUGUI waveCounter;
    [HideInInspector]public bool active = false;
    public Shop shop;

    private void Awake()
    {
        lanes = FindObjectsOfType<LaneSpawner>();
        active = false;
        StartWave();
    }

    void InitializeWave(int numWave)
    {
        for(int i  = 0; i <lanes.Length; i++)
        {
            lanes[i].waves = new List<Subwave>();       
        }

        Wave current = (currentWave < waves.Count) ? waves[numWave] : waves[waves.Count - 1];

        foreach (Subwave s in current.subwaves)
        {
            lanes[Random.Range(0, lanes.Length)].waves.Add(s);
        }
    }

    void Update()
    {
        if (winState)
        {
            if (Input.anyKeyDown){
                winScreen.DOFade(0, .2f);
                shop.OpenShop();
                winState = false;
            }
        }

        if (active)
        {
            waveTime += Time.deltaTime;
            foreach (LaneSpawner l in lanes)
            {
                l.TrySpawn(waveTime);
            }
        }

        if(active)
            CheckWaveEnd();
    }

    public void CheckWaveEnd()
    {
        bool completed = true;
        foreach(LaneSpawner l in lanes)
        {
            if (l.GetLaneCompletion() == false)
                completed = false;
        }

        

        if (EnemyManager.instance.enemies.Count > 0)
            completed = false;


        if (completed)
            EndWave();
    }
    public CanvasGroup winScreen;
    bool winState = false;
    public void EndWave()
    {
        Tower.instance.princess.transform.position = new Vector3(Tower.instance.climbingTop.position.x, Tower.instance.climbingTop.position.y,0);
        Tower.instance.activeWave = false;
        Tower.instance.princess.transform.SetParent(Tower.instance.towerTop);
        waveTime = 0;
        active = false;
        currentWave++;
        
        if(currentWave != 9)
        {
            FindObjectOfType<WaveTransition>().EndWave(shop.OpenShop);
        } else
        {
            
            Sequence s = DOTween.Sequence();
            s.Append(winScreen.DOFade(1, .25f));
            s.AppendInterval(1);
            s.AppendCallback(() => { winState = true; });
        }

        
    }

    public void StartWave()
    {
        foreach (PlayerUnit p in FindObjectsOfType<PlayerUnit>())
        {
            p.Refresh();
        }
        waveCounter.text = "Wave " + (currentWave+1);
       
        shop.gameObject.SetActive(false);
        FindObjectOfType<WaveTransition>().StartWave(LoadWave);
        
    }

    void LoadWave()
    {
        InitializeWave(currentWave);
        Tower.instance.activeWave = true; 
        active = true; 
    }
}
