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

    // 게임 상태 열거형
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

        // 타이머가 할당 안 됐으면 씬에서 찾아 할당
        if (timer == null)
        {
            timer = Object.FindFirstObjectByType<Timer>();
            if (timer == null)
                Debug.LogWarning("Timer 컴포넌트를 씬에서 찾지 못했습니다.");
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

                // 모든 적 스포너들 멈추기
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

                // 모든 코인 삭제
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
        Debug.Log("상태: Idle - 게임 대기 중");
        Time.timeScale = 1f;
        if (shopUI != null) shopUI.SetActive(false);
    }

    public void ChangeStateToStart()
    {
        currentState = GameState.Start;
        Debug.Log("상태: Start - 게임 준비 중");
        Time.timeScale = 1f;
        if (shopUI != null) shopUI.SetActive(false);
    }

    public void ChangeStateToGame()
    {
        currentState = GameState.Game;
        Debug.Log("상태: Game - 웨이브 진행 중");

        diceAnimation.StartRollingLoop(); // 주사위 애니메이션 시작

        // 모든 일반 적 스포너 시작
        EnemySpawner[] enemySpawners = Object.FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
        foreach (var spawner in enemySpawners)
        {
            spawner.StartSpawning();
        }

        // 모든 돌진 적 스포너 시작
        DashEnemySpawner[] dashSpawners = Object.FindObjectsByType<DashEnemySpawner>(FindObjectsSortMode.None);
        foreach (var spawner in dashSpawners)
        {
            spawner.StartSpawning();
        }

        // 모든 원거리 적 스포너 시작
        LongRangeEnemySpawner[] longRangeSpawners = Object.FindObjectsByType<LongRangeEnemySpawner>(FindObjectsSortMode.None);
        foreach (var spawner in longRangeSpawners)
        {
            spawner.StartSpawning();
        }

        // 모든 물약 적 스포너 시작
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
        Debug.Log("상태: Shop - 상점 상태");

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
        Debug.Log("상태: End - 게임 오버");
        Time.timeScale = 0f;
        if (shopUI != null) shopUI.SetActive(false);
    }

    public void ChangeStateToClear()
    {
        currentState = GameState.Clear;
        Debug.Log("상태: Clear - 모든 웨이브 클리어");
        Time.timeScale = 0f;
        if (shopUI != null) shopUI.SetActive(false);
    }

    // 상태 확인용 유틸리티
    public bool IsIdle() => currentState == GameState.Idle;
    public bool IsStart() => currentState == GameState.Start;
    public bool IsGame() => currentState == GameState.Game;
    public bool IsShop() => currentState == GameState.Shop;
    public bool IsEnd() => currentState == GameState.End;
    public bool IsClear() => currentState == GameState.Clear;
}
