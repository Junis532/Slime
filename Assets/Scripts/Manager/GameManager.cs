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
    public EnemyHP enemyHP;
    public DiceAnimation diceAnimation;
    public FireballProjectile fireballProjectile;
    public LightningDamage lightningDamage;

    [Header("UI")]
    public GameObject shopUI;

    [Header("���� �г�")]
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

                // ������ ���� ����
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

                // �� ���� �� ���� ���� �ڷ�ƾ ����
                StartCoroutine(WaitForEnemiesDieAndGoShop());
            }
        }
    }

    private IEnumerator WaitForEnemiesDieAndGoShop()
    {
        // ��� ���鿡�� ������� ���
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

        // ��� ���� ����
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        foreach (GameObject coin in coins)
        {
            Destroy(coin);
        }

        // ��� �Ѿ� ����
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            Destroy(bullet);
        }

        // ��� ��ų ����
        GameObject[] skills = GameObject.FindGameObjectsWithTag("Skill");
        foreach (GameObject skill in skills)
        {
            Destroy(skill);
        }

        // ������ Destroy�� ������ ��� (�ִ� 2�� ���)
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

        // �� �� ���� �� �������� ��ȯ
        ChangeStateToShop();
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

        diceAnimation.StartRollingLoop();

        EnemySpawner[] enemySpawners = Object.FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
        foreach (var spawner in enemySpawners)
        {
            spawner.StartSpawning();
        }

        DashEnemySpawner[] dashSpawners = Object.FindObjectsByType<DashEnemySpawner>(FindObjectsSortMode.None);
        foreach (var spawner in dashSpawners)
        {
            spawner.StartSpawning();
        }

        LongRangeEnemySpawner[] longRangeSpawners = Object.FindObjectsByType<LongRangeEnemySpawner>(FindObjectsSortMode.None);
        foreach (var spawner in longRangeSpawners)
        {
            spawner.StartSpawning();
        }

        PotionEnemySpawner[] potionSpawners = Object.FindObjectsByType<PotionEnemySpawner>(FindObjectsSortMode.None);
        foreach (var spawner in potionSpawners)
        {
            spawner.StartSpawning();
        }

        //if (shopUI != null) shopUI.SetActive(false);
    }

    public void ChangeStateToShop()
    {
        currentState = GameState.Shop;
        Debug.Log("����: Shop - ���� ����");
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
                canvasGroup.DOFade(1f, 0.7f);  // 0f = ���� ����, 0.5�� ����
            }
            // �ε巴�� X=0���� �̵� �� Ÿ�ӽ����� 0���� ����
            shopPanel.DOAnchorPosX(0f, 0.7f)
                .SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    //Time.timeScale = 0f;
                });
        }
        else
        {
            // shopPanel�� null�� ��� Ÿ�ӽ����� �ٷ� 0���� ó��
            Time.timeScale = 0f;
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

    public bool IsIdle() => currentState == GameState.Idle;
    public bool IsStart() => currentState == GameState.Start;
    public bool IsGame() => currentState == GameState.Game;
    public bool IsShop() => currentState == GameState.Shop;
    public bool IsEnd() => currentState == GameState.End;
    public bool IsClear() => currentState == GameState.Clear;
}
