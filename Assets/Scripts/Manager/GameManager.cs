using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoSigleTone<GameManager>
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

    [System.Obsolete]
    protected new void Awake()
    {
        base.Awake();

        // Ÿ�̸Ӱ� �Ҵ� �� ������ ������ ã�� �Ҵ�
        if (timer == null)
        {
            timer = FindObjectOfType<Timer>();
            if (timer == null)
                Debug.LogWarning("Timer ������Ʈ�� ������ ã�� ���߽��ϴ�.");
        }
    }

    [System.Obsolete]
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

    [System.Obsolete]
    void Update()
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
                EnemySpawner[] enemySpawners = FindObjectsOfType<EnemySpawner>();
                foreach (var spawner in enemySpawners)
                {
                    spawner.StopSpawning();
                }

                DashEnemySpawner[] dashSpawners = FindObjectsOfType<DashEnemySpawner>();
                foreach (var spawner in dashSpawners)
                {
                    spawner.StopSpawning();
                }

                LongRangeEnemySpawner[] longRangeSpawners = FindObjectsOfType<LongRangeEnemySpawner>();
                foreach (var spawner in longRangeSpawners)
                {
                    spawner.StopSpawning();
                }

                PotionEnemySpawner[] potionSpawners = FindObjectsOfType<PotionEnemySpawner>();
                foreach (var spawner in potionSpawners)
                {
                    spawner.StopSpawning();
                }


                //EnemySpawner[] spawners = FindObjectsOfType<EnemySpawner>();
                //foreach (var spawner in spawners)
                //{
                //    spawner.StopSpawning();
                //}
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

                        // ��� ���� ����
                        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
                        foreach (GameObject coin in coins)
                        {
                            Destroy(coin);
                        }
                    }
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

    [System.Obsolete]
    public void ChangeStateToGame()
    {
        currentState = GameState.Game;
        Debug.Log("����: Game - ���̺� ���� ��");

        // ��� �Ϲ� �� ������ ����
        EnemySpawner[] enemySpawners = FindObjectsOfType<EnemySpawner>();
        foreach (var spawner in enemySpawners)
        {
            spawner.StartSpawning();
        }

        // ��� ���� �� ������ ����
        DashEnemySpawner[] dashSpawners = FindObjectsOfType<DashEnemySpawner>();
        foreach (var spawner in dashSpawners)
        {
            spawner.StartSpawning();
        }

        // ��� ���Ÿ� �� ������ ����
        LongRangeEnemySpawner[] longRangeSpawners = FindObjectsOfType<LongRangeEnemySpawner>();
        foreach (var spawner in longRangeSpawners)
        {
            spawner.StartSpawning();
        }

        // ��� ���� �� ������ ����
        PotionEnemySpawner[] potionSpawners = FindObjectsOfType<PotionEnemySpawner>();
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
        shopManager.FirstRerollItems();
        if (timer != null)
        {
            timer.ResetTimer(0f);
        }
        Time.timeScale = 0f;

        if (shopUI != null)
        {
            shopUI.SetActive(true);

            //// ShopManager �ʱ�ȭ ȣ��
            //ShopManager shopManager = shopUI.GetComponent<ShopManager>();
            //if (shopManager != null)
            //{
            //    shopManager.InitShopUI();
            //}
            //else
            //{
            //    Debug.LogWarning("ShopUI�� ShopManager ������Ʈ�� �����ϴ�.");
            //}
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
