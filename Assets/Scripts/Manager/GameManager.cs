using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleTone<GameManager>
{
    public PlayerDamaged playerDamaged;
    public PlayerStats playerStats;
    public EnemyStats enemyStats;
    public DashEnemyStats dashEnemyStats;
    public LongRangeEnemyStats longRangeEnemyStats;
    public PotionEnemyStats potionEnemyStats;
    public EnemySpawner enemySpawner;
    public DashEnemySpawner dashEnemySpawner;
    public LongRangeEnemySpawner longRangeEnemySpawner;
    public PotionEnemySpawner potionEnemySpawner;
    public ItemStats itemStats1;
    public ItemStats itemStats2;
    public ItemStats itemStats3;
    public ItemStats itemStats4;
    public ItemStats itemStats5;
    public ItemStats itemStats6;
    public Enemy enemy;
    public DashEnemy dashEnemy;
    public LongRangeEnemy longRangeEnemy;
    public PotionEnemy potionEnemy;
    public Timer timer;
    public ShopManager shopManager;
    public WaveManager waveManager;
    public DialogManager dialogManager;
    public SpawnerManager spawnerManager;
    public EnemyHP enemyHP;
    public DiceAnimation diceAnimation;
    public FireballProjectile fireballProjectile;
    public LightningDamage lightningDamage;
    public ZacSkill zacSkill;

    [Header("UI")]
    public GameObject shopUI;

    [Header("상점 패널")]
    public RectTransform shopPanel;


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

                // 스포너 전부 정지
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

        // 모든 코인 삭제
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        foreach (GameObject coin in coins)
        {
            Destroy(coin);
        }

        // 모든 총알 삭제
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            Destroy(bullet);
        }

        // 모든 스킬 삭제
        GameObject[] skills = GameObject.FindGameObjectsWithTag("Skill");
        foreach (GameObject skill in skills)
        {
            Destroy(skill);
        }

        // 적들이 Destroy될 때까지 대기 (최대 2초 대기)
        float waitTime = 0f;
        float maxWaitTime = 2f;

        while (waitTime < maxWaitTime)
        {
            bool anyEnemyLeft = false;
            foreach (string tag in enemyTags)
            {
                if (GameObject.FindGameObjectsWithTag(tag).Length > 0)
                {
                    anyEnemyLeft = true;
                    break;
                }
            }

            if (!anyEnemyLeft)
                break;

            waitTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // 적 다 죽은 후 상점으로 전환
        ChangeStateToShop();
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

        diceAnimation.StartRollingLoop();

        if (spawnerManager != null)
        {
            // 코루틴으로 자동 반복 스폰 시작
            // SpawnSpawnersAroundPlayer()를 일정 주기로 반복 호출
            // 이미 SpawnerManager 내부에서 StartCoroutine이 Start()에서 자동 실행되므로 따로 호출 안해도 됨
            // 만약 수동 시작 필요하면 아래 주석 해제:
            spawnerManager.StartCoroutine("SpawnerLoopRoutine");
        }


        //EnemySpawner[] enemySpawners = Object.FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
        //foreach (var spawner in enemySpawners)
        //{
        //    spawner.StartSpawning();
        //}

        //DashEnemySpawner[] dashSpawners = Object.FindObjectsByType<DashEnemySpawner>(FindObjectsSortMode.None);
        //foreach (var spawner in dashSpawners)
        //{
        //    spawner.StartSpawning();
        //}

        //LongRangeEnemySpawner[] longRangeSpawners = Object.FindObjectsByType<LongRangeEnemySpawner>(FindObjectsSortMode.None);
        //foreach (var spawner in longRangeSpawners)
        //{
        //    spawner.StartSpawning();
        //}

        //PotionEnemySpawner[] potionSpawners = Object.FindObjectsByType<PotionEnemySpawner>(FindObjectsSortMode.None);
        //foreach (var spawner in potionSpawners)
        //{
        //    spawner.StartSpawning();
        //}

        //if (shopUI != null) shopUI.SetActive(false);
    }

    public void ChangeStateToShop()
    {
        currentState = GameState.Shop;
        Debug.Log("상태: Shop - 상점 상태");
        DialogManager.Instance.StartShopDialog();

        diceAnimation.StopRollingLoop();

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

    public bool IsIdle() => currentState == GameState.Idle;
    public bool IsStart() => currentState == GameState.Start;
    public bool IsGame() => currentState == GameState.Game;
    public bool IsShop() => currentState == GameState.Shop;
    public bool IsEnd() => currentState == GameState.End;
    public bool IsClear() => currentState == GameState.Clear;
}
