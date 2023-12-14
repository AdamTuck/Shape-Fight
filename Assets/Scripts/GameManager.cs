using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    LevelLoader levelLoader;
    PickupSpawner pickupSpawner;
    UIManager uiManager;

    private static GameManager instance;
    public ScoreManager scoreManager;
    private Player player;

    private bool isPlaying;

    public Action OnGameStart, OnGameOver;

    private GameObject tempEnemy;
    private bool isEnemySpawning;

    private int currentLevel;
    private float currentLevelTimer;

    private Weapon meleeWeapon = new Weapon("Melee", 1, 0);
    private Weapon machineGunWeapon = new Weapon("Machine Gun", 2, 5);
    private Weapon sniperWeapon = new Weapon("Sniper", 5, 15);

    [Header("Game Objects")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject meleeEnemyPrefab, machineGunEnemyPrefab, sniperEnemyPrefab;
    [SerializeField] private Transform[] spawnPositions;

    [Header("Enemy Details")]
    [SerializeField] private float enemySpawnRate;
    [SerializeField] private int difficultyLevel;

    [Header("Level Details")]
    [SerializeField] private float levelLength;

    public static GameManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance && instance != this)
        {
            Destroy(this);
        }

        instance = this;

        currentLevel = 1;
    }

    private void Update()
    {
        LevelManager();
    }

    // Start is called before the first frame update
    void Start()
    {
        levelLoader = FindObjectOfType<LevelLoader>();
        scoreManager = FindObjectOfType<ScoreManager>();
        pickupSpawner = FindObjectOfType<PickupSpawner>();
        uiManager = FindObjectOfType<UIManager>();
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }

    void CreateMeleeEnemy ()
    {
        tempEnemy = Instantiate(meleeEnemyPrefab);
        tempEnemy.transform.position = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Length)].position;

        tempEnemy.GetComponent<Enemy>().weapon = meleeWeapon;
        tempEnemy.GetComponent<MeleeEnemy>().SetMeleeEnemy(2, 0.25f);
    }

    void CreateMachineGunEnemy()
    {
        tempEnemy = Instantiate(machineGunEnemyPrefab);
        tempEnemy.transform.position = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Length)].position;

        tempEnemy.GetComponent<Enemy>().weapon = machineGunWeapon;
        tempEnemy.GetComponent<MachineGunEnemy>().SetMachineGunEnemy(8, 0.6f);
    }

    void CreateSniperEnemy()
    {
        tempEnemy = Instantiate(sniperEnemyPrefab);
        tempEnemy.transform.position = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Length)].position;

        tempEnemy.GetComponent<Enemy>().weapon = sniperWeapon;
        tempEnemy.GetComponent<SniperEnemy>().SetSniperEnemy(15, 4f);
    }

    IEnumerator EnemySpawner ()
    {
        while(isEnemySpawning)
        {
            yield return new WaitForSeconds(1.0f / enemySpawnRate);
            ChooseEnemyToSpawn();
        }
    }

    public void NotifyDeath(Enemy enemy)
    {
        pickupSpawner.SpawnPickup(enemy.transform.position);
    }

    public Player GetPlayer ()
    {
        return player;
    }

    public void StopEnemySpawning()
    {
        isEnemySpawning = false;
    }

    public void StartGame()
    {
        player = Instantiate(playerPrefab, Vector2.zero, Quaternion.identity).GetComponent<Player>();
        player.OnDeath += StopGame;
        isPlaying = true;

        OnGameStart?.Invoke();
        StartCoroutine(GameStarter());
    }

    public void StopGame()
    {
        scoreManager.SetHighScore();

        StartCoroutine(GameStopper());
    }

    IEnumerator GameStopper()
    {
        isEnemySpawning = false;
        yield return new WaitForSeconds(2.0f);
        isPlaying = false;

        WipeScreen();

        OnGameOver?.Invoke();
    }

    IEnumerator GameStarter()
    {
        yield return new WaitForSeconds(2.0f);

        isEnemySpawning = true;
        StartCoroutine(EnemySpawner());

        OnGameStart?.Invoke();
    }

    private static void WipeScreen()
    {
        // Delete all enemies
        foreach (Enemy item in FindObjectsOfType(typeof(Enemy)))
        {
            Destroy(item.gameObject);
        }

        foreach (Pickup item in FindObjectsOfType(typeof(Pickup)))
        {
            Destroy(item.gameObject);
        }

        foreach (Bullet item in FindObjectsOfType(typeof(Bullet)))
        {
            Destroy(item.gameObject);
        }
    }

    private void LevelManager()
    {
        if (isPlaying)
            currentLevelTimer += Time.deltaTime;

        if (currentLevelTimer >= levelLength)
        {
            WipeScreen();
            currentLevel++;
            currentLevelTimer = 0;
            enemySpawnRate += 0.1f * difficultyLevel;

            uiManager.ShowLevel(currentLevel);
        }
    }

    private void ChooseEnemyToSpawn ()
    {
        float randomSpawnValue = UnityEngine.Random.Range(0f, 100f);

        randomSpawnValue += currentLevel * difficultyLevel * 5;

        if (randomSpawnValue >= 100)
        {
            CreateSniperEnemy();
        }
        else if (randomSpawnValue >= 80)
        {
            CreateMachineGunEnemy();
        }
        else
        {
            CreateMeleeEnemy();
        }
    }
}