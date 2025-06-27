using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleTone<GameManager>
{
    public PlayerStats playerStats;
    public EnemyStats enemyStats;
    public DashEnemyStats dashEnemyStats;
    public LongRangeEnemyStats longRangeEnemyStats;
    public PotionEnemyStats potionEnemyStats;
    public EnemySpawner enemySpawner;
    public DashEnemySpawner dashEnemySpawner;
    public LongRangeEnemySpawner longRangeEnemySpawner;
    public PotionEnemySpawner potionEnemySpawner;
    public Enemy enemy;
    public DashEnemy dashEnemy;
    public LongRangeEnemy longRangeEnemy;
    public PotionEnemy potionEnemy;
    public Timer timer;
    public ShopManager shopManager;
    public WaveManager waveManager;
    public EnemyHP enemyHP;
    public DiceAnimation diceAnimation;
    public FireballProjectile fireballProjectile;
    public LightningDamage lightningDamage;

    [Header("UI")]
    public GameObject shopUI;

    public int currentWave = 1;

    // ���� ���� ������
    private enum GameState
    {
        Idle,
        Start,
        Game,
        Shop,
        End,
        Clear
    }

    private GameState currentState = GameState.Idle;
    public string CurrentState => currentState.ToString();

    protected new void Awake()
    {
        base.Awake();

        // Ÿ�̸Ӱ� �Ҵ� �� ������ ������ ã�� �Ҵ�
        if (timer == null)
        {
            timer = Object.FindFirstObjectByType<Timer>();
            if (timer == null)
                Debug.LogWarning("Timer ������Ʈ�� ������ ã�� ���߽��ϴ�.");
        }
    }

    private void Start()
    {
        ChangeStateToGame();
        playerStats.ResetStats();
        enemyStats.ResetStats();
        dashEnemyStats.ResetStats();
        longRangeEnemyStats.ResetStats();
        potionEnemyStats.ResetStats();
        waveManager.UpdateWaveText();
    }

    private void Update()
    {
        if (currentState == GameState.Game && timer != null && timer.timerRunning)
        {
            if (timer.timeRemaining > 0)
            {
                timer.timeRemaining -= Time.deltaTime;
                timer.UpdateTimerDisplay();
            }
            else
            {
                timer.timeRemaining = 0;
                timer.timerRunning = false;
                timer.UpdateTimerDisplay();

                // ��� �� �����ʵ� ���߱�
                EnemySpawner[] enemySpawners = Object.FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
                foreach (var spawner in enemySpawners)
                {
                    spawner.StopSpawning();
                }

                DashEnemySpawner[] dashSpawners = Object.FindObjectsByType<DashEnemySpawner>(FindObjectsSortMode.None);
                foreach (var spawner in dashSpawners)
                {
                    spawner.StopSpawning();
                }

                LongRangeEnemySpawner[] longRangeSpawners = Object.FindObjectsByType<LongRangeEnemySpawner>(FindObjectsSortMode.None);
                foreach (var spawner in longRangeSpawners)
                {
                    spawner.StopSpawning();
                }

                PotionEnemySpawner[] potionSpawners = Object.FindObjectsByType<PotionEnemySpawner>(FindObjectsSortMode.None);
                foreach (var spawner in potionSpawners)
                {
                    spawner.StopSpawning();
                }

                string[] enemyTags = { "Enemy", "DashEnemy", "LongRangeEnemy", "PotionEnemy" };

                foreach (string tag in enemyTags)
                {
                    GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
                    foreach (GameObject enemyObject in enemies)
                    {
                        if (tag == "Enemy")
                        {
                            Enemy enemy = enemyObject.GetComponent<Enemy>();
                            if (enemy != null)
                            {
                                enemy.Die();
                            }
                        }
                        else if (tag == "DashEnemy")
                        {
                            DashEnemy dashEnemy = enemyObject.GetComponent<DashEnemy>();
                            if (dashEnemy != null)
                            {
                                dashEnemy.Die();
                            }
                        }
                        else if (tag == "LongRangeEnemy")
                        {
                            LongRangeEnemy longRangeEnemy = enemyObject.GetComponent<LongRangeEnemy>();
                            if (longRangeEnemy != null)
                            {
                                longRangeEnemy.Die();
                            }
                        }
                        else if (tag == "PotionEnemy")
                        {
                            PotionEnemy potionEnemy = enemyObject.GetComponent<PotionEnemy>();
                            if (potionEnemy != null)
                            {
                                potionEnemy.Die();
                            }
                        }
                    }
                }

                // ��� ���� ����
                GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
                foreach (GameObject coin in coins)
                {
                    Destroy(coin);
                }

                ChangeStateToShop();
            }
        }
    }

    public void ChangeStateToIdle()
    {
        currentState = GameState.Idle;
        Debug.Log("����: Idle - ���� ��� ��");
        Time.timeScale = 1f;
        if (shopUI != null) shopUI.SetActive(false);
    }

    public void ChangeStateToStart()
    {
        currentState = GameState.Start;
        Debug.Log("����: Start - ���� �غ� ��");
        Time.timeScale = 1f;
        if (shopUI != null) shopUI.SetActive(false);
    }

    public void ChangeStateToGame()
    {
        currentState = GameState.Game;
        Debug.Log("����: Game - ���̺� ���� ��");

        diceAnimation.StartRollingLoop(); // �ֻ��� �ִϸ��̼� ����

        // ��� �Ϲ� �� ������ ����
        EnemySpawner[] enemySpawners = Object.FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
        foreach (var spawner in enemySpawners)
        {
            spawner.StartSpawning();
        }

        // ��� ���� �� ������ ����
        DashEnemySpawner[] dashSpawners = Object.FindObjectsByType<DashEnemySpawner>(FindObjectsSortMode.None);
        foreach (var spawner in dashSpawners)
        {
            spawner.StartSpawning();
        }

        // ��� ���Ÿ� �� ������ ����
        LongRangeEnemySpawner[] longRangeSpawners = Object.FindObjectsByType<LongRangeEnemySpawner>(FindObjectsSortMode.None);
        foreach (var spawner in longRangeSpawners)
        {
            spawner.StartSpawning();
        }

        // ��� ���� �� ������ ����
        PotionEnemySpawner[] potionSpawners = Object.FindObjectsByType<PotionEnemySpawner>(FindObjectsSortMode.None);
        foreach (var spawner in potionSpawners)
        {
            spawner.StartSpawning();
        }

        Time.timeScale = 1f;
        if (shopUI != null) shopUI.SetActive(false);
    }

    public void ChangeStateToShop()
    {
        currentState = GameState.Shop;
        Debug.Log("����: Shop - ���� ����");

        diceAnimation.StopRollingLoop();

        shopManager.FirstRerollItems();
        if (timer != null)
        {
            timer.ResetTimer(10f);
        }
        Time.timeScale = 0f;

        if (shopUI != null)
        {
            shopUI.SetActive(true);
        }
    }

    public void ChangeStateToEnd()
    {
        currentState = GameState.End;
        Debug.Log("����: End - ���� ����");
        Time.timeScale = 0f;
        if (shopUI != null) shopUI.SetActive(false);
    }

    public void ChangeStateToClear()
    {
        currentState = GameState.Clear;
        Debug.Log("����: Clear - ��� ���̺� Ŭ����");
        Time.timeScale = 0f;
        if (shopUI != null) shopUI.SetActive(false);
    }

    // ���� Ȯ�ο� ��ƿ��Ƽ
    public bool IsIdle() => currentState == GameState.Idle;
    public bool IsStart() => currentState == GameState.Start;
    public bool IsGame() => currentState == GameState.Game;
    public bool IsShop() => currentState == GameState.Shop;
    public bool IsEnd() => currentState == GameState.End;
    public bool IsClear() => currentState == GameState.Clear;
}
