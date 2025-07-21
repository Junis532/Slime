using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleTone<GameManager>
{
    public PlayerDamaged playerDamaged;
    public PlayerDie playerDie;
    public PlayerStats playerStats;
    public EnemyStats enemyStats;
    public DashEnemyStats dashEnemyStats;
    public LongRangeEnemyStats longRangeEnemyStats;
    public PotionEnemyStats potionEnemyStats;
    public EnemySpawner enemySpawner;
    public ItemStats itemStats1;
    public ItemStats itemStats2;
    public ItemStats itemStats3;
    public ItemStats itemStats4;
    public ItemStats itemStats5;
    public ItemStats itemStats6;
    public ItemStats itemStats7;
    public ItemStats itemStats8;
    public Enemy enemy;
    public DashEnemy dashEnemy;
    public LongRangeEnemy longRangeEnemy;
    public PotionEnemy potionEnemy;
    public Timer timer;
    public AudioManager audioManager;
    public PoolManager poolManager;
    public ShopManager shopManager;
    public WaveManager waveManager;
    public DialogManager dialogManager;
    public EnemyHP enemyHP;
    public DiceAnimation diceAnimation;
    //public FireballProjectile fireballProjectile;
    //public LightningDamage lightningDamage;
    public ZacSkill zacSkill;

    [Header("UI")]
    public GameObject shopUI;

    [Header("상점 패널")]
    public RectTransform shopPanel;


    private enum GameState
    {
        Lobby,
        Game,
        Shop,
        End,
        Clear
    }

    private GameState currentState = GameState.Lobby;
    public string CurrentState => currentState.ToString();

    protected new void Awake()
    {
        // VSync 비활성화 (모니터 주사율 영향 제거)
        QualitySettings.vSyncCount = 0;

        // 프레임 고정
        Application.targetFrameRate = 60;

        // 중복 GameManager 방지
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        base.Awake();

        if (timer == null)
        {
            timer = Object.FindFirstObjectByType<Timer>();
            if (timer == null)
                Debug.LogWarning("Timer 컴포넌트를 씬에서 찾지 못했습니다.");
        }
    }

    private void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Lobby") // 로비 씬일 경우
        {
            ChangeStateToIdle();
            return;
        }
        else if (sceneName == "InGame") // 게임 씬일 경우
        {
            ChangeStateToGame();
        }

        playerStats.ResetStats();
        enemyStats.ResetStats();
        dashEnemyStats.ResetStats();
        longRangeEnemyStats.ResetStats();
        potionEnemyStats.ResetStats();
        waveManager.UpdateWaveText();


        if (shopUI != null)
        {
            CanvasGroup canvasGroup = shopPanel.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            shopUI.SetActive(false);
        }
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

                // 적 죽음 및 상점 진입 코루틴 시작
                StartCoroutine(WaitForEnemiesDieAndGoShop());
            }
        }
    }

    private IEnumerator WaitForEnemiesDieAndGoShop()
    {
        // 모든 적들에게 죽으라고 명령
        string[] enemyTags = { "Enemy", "DashEnemy", "LongRangeEnemy", "PotionEnemy" };

        foreach (string tag in enemyTags)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject enemyObject in enemies)
            {
                EnemiesDie enemiesDie = enemyObject.GetComponent<EnemiesDie>();
                if (enemiesDie != null)
                {
                    enemiesDie.Die();
                }
            }
        }

        // 코인, 총알, 스킬 삭제 부분 그대로
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        foreach (GameObject coin in coins)
        {
            PoolManager.Instance.ReturnToPool(coin);
        }
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            PoolManager.Instance.ReturnToPool(bullet);
        }
        GameObject[] skills = GameObject.FindGameObjectsWithTag("Skill");
        foreach (GameObject skill in skills)
        {
            Destroy(skill);
        }

        // 기다리지 않고 바로 상점 상태로 전환
        ChangeStateToShop();
        yield break;
    }


    public void ChangeStateToIdle()
    {
        currentState = GameState.Lobby;
        Debug.Log("상태: Lobby - 게임 대기 중");
        Time.timeScale = 1f;
        if (shopUI != null) shopUI.SetActive(false);
    }

    public void ChangeStateToGame()
    {
        currentState = GameState.Game;
        Debug.Log("상태: Game - 웨이브 진행 중");

        diceAnimation.StartRollingLoop();

        waveManager.StartSpawnLoop();

    }

    public void ChangeStateToShop()
    {
        currentState = GameState.Shop;
        Debug.Log("상태: Shop - 상점 상태");
        DialogManager.Instance.StartShopDialog();

        diceAnimation.StopRollingLoop();

        waveManager.StopSpawnLoop();

        shopManager.FirstRerollItems();

        if (timer != null)
        {
            timer.ResetTimer(60f);
        }

        if (shopUI != null)
        {
            shopUI.SetActive(true);
        }

        if (shopPanel != null)
        {

            CanvasGroup canvasGroup = shopPanel.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.DOFade(1f, 0.7f);  // 0f = 완전 투명, 0.5초 동안
            }
            // 부드럽게 X=0으로 이동 후 타임스케일 0으로 변경
            shopPanel.DOAnchorPosY(0f, 0.7f)
                .SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    //Time.timeScale = 0f;
                });
        }
        else
        {
            // shopPanel이 null일 경우 타임스케일 바로 0으로 처리
            Time.timeScale = 0f;
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

    public bool IsIdle() => currentState == GameState.Lobby;
    public bool IsGame() => currentState == GameState.Game;
    public bool IsShop() => currentState == GameState.Shop;
    public bool IsEnd() => currentState == GameState.End;
    public bool IsClear() => currentState == GameState.Clear;
}