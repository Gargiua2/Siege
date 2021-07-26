using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;
using System;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class Tower : MonoBehaviour, IDamagable
{
    public CanvasGroup g;
    public TextMeshProUGUI waveNum;
    public LayerMask wall;

    public Transform climbingBottom;
    public Transform climbingTop;
    public Transform laneEnd;
    public Transform wallAttackPos;

    public Transform towerTop;
    public Tilemap middleTower;
    public Tile leftTowerWall;
    public Tile rightTowerWall;
    public Tile vineWall;

    public int maxExtraLayers = 5;

    public Vector2 minArcherArea;
    public Vector2 maxArcherArea;

    [HideInInspector]public int extraLayers = 0;

    public Action OnTowerDrop;
    public GameObject fireParticles;
    public GameObject forzenParticles;
    public GameObject stunParticles;

    [Header("STAT COSTS")]
    public int[] attackCosts;
    public int[] staminaCosts;
    public int[] speedCosts;


    public List<PlayerUnit> attackableUnits;
    [HideInInspector] public bool activeWave = false;

    public float maxHP;
    float currentHP;
    public List<ParticleSystem> onDamagedParticles = new List<ParticleSystem>();

    float mostRecentClimber = 0;

    public Princess princess;

    public TextMeshProUGUI coinCounter;
    [HideInInspector]public int coinCount;
    [HideInInspector] public LaneSpawner[] lanes;

    [Header("AUDIO")]
    public AudioSource audioSource;
    public AudioClip towerInteract;
    public AudioClip damage;

    bool canRestart = false;

    #region Singleton
    public static Tower instance;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }    
    }
    #endregion

    void Start()
    {
        lanes = FindObjectsOfType<LaneSpawner>();
        currentHP = maxHP;
    }

    bool gameOver = false;
    public void GameOver()
    {
        gameOver = true;
        waveNum.text = "You reached wave " + (FindObjectOfType<WaveManager>().currentWave + 1) + "\n Better luck next time!";
    }

    public void PlayDamageSFX()
    {
        audioSource.PlayOneShot(damage,.5f);
    }

    void Update()
    {
        mostRecentClimber += Time.deltaTime;

        if(gameOver && g.alpha < 1)
        {
            g.alpha += .06f;
        }

        if (g.alpha > .95f && Input.anyKeyDown)
        {
            SceneManager.LoadScene(0);
        }
    }

    public bool TryStartClimb()
    {
        if (mostRecentClimber < .15f)
            return false;

        mostRecentClimber = 0;
        return true;
    }

    public void AddCoins(int amount)
    {
        coinCount += amount;
        coinCounter.text = "x" + coinCount;
    }

    public bool Grow()
    {
        audioSource.PlayOneShot(towerInteract);

        if (extraLayers + 1 > maxExtraLayers)
            return false;

        Vector3Int startTile = new Vector3Int(4, -2 + extraLayers, 0);
        middleTower.SetTile(startTile, leftTowerWall);
        middleTower.SetTile(startTile + Vector3Int.right, vineWall);
        middleTower.SetTile(startTile + Vector3Int.right * 2, vineWall);
        middleTower.SetTile(startTile + Vector3Int.right * 3, rightTowerWall);
        towerTop.transform.position += Vector3.up;
        extraLayers++;
        return true;
    }

    public void Fall()
    {
        audioSource.PlayOneShot(towerInteract);
        if (extraLayers == 0)
            return;

        extraLayers--;
        Vector3Int startTile = new Vector3Int(4, -2 + extraLayers, 0);
        middleTower.SetTile(startTile, null);
        middleTower.SetTile(startTile + Vector3Int.right, null);
        middleTower.SetTile(startTile + Vector3Int.right * 2, null);
        middleTower.SetTile(startTile + Vector3Int.right * 3, null);
        towerTop.transform.position -= Vector3.up;
        OnTowerDrop?.Invoke();
    }

    public Vector2 GetRandomPointAlongWall()
    {
        Vector2 wallCenter = wallAttackPos.position.Flat();

        return Vector2.Lerp(wallCenter - Vector2.up * .75f, wallCenter + Vector2.up * .75f, UnityEngine.Random.value);
    }

    public Vector2 GetRandomArcherPoint()
    {
        return new Vector2(Mathf.Lerp(minArcherArea.x, maxArcherArea.x, UnityEngine.Random.value), Mathf.Lerp(minArcherArea.y, maxArcherArea.y, UnityEngine.Random.value));
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(laneEnd.position - Vector3.up * 3, laneEnd.position + Vector3.up * 3);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(climbingBottom.position, .25f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(climbingTop.position, .25f);

        Gizmos.color = Color.black;
        Gizmos.DrawLine(wallAttackPos.position - Vector3.up * .75f, wallAttackPos.position + Vector3.up * .75f);

        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3((minArcherArea.x + maxArcherArea.x) / 2, (minArcherArea.y + maxArcherArea.y) / 2,0);
        Vector3 boundes = new Vector3(maxArcherArea.x - minArcherArea.x, maxArcherArea.y - minArcherArea.y);
        Gizmos.DrawWireCube(center, boundes);
    }

    public void ReceieveDamage(float damage)
    {
        currentHP -= damage;
        onDamagedParticles[UnityEngine.Random.Range(0, onDamagedParticles.Count)].Play();

        if(currentHP <= 0)
        {
            Fall();
            currentHP = maxHP;
        }
    }
}

public interface IDamagable
{
    void ReceieveDamage(float damage);
}