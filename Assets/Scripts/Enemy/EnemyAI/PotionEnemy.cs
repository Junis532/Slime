using DG.Tweening;
using UnityEngine;

public class PotionEnemy : EnemyBase
{
    private bool isLive = true;

    private SpriteRenderer spriter;
    private EnemyAnimation enemyAnimation;

    private Vector2 currentVelocity;
    private Vector2 currentDirection;

    public float smoothTime = 0.1f;

    public float stopCooldown = 3f;         // 멈추는 주기
    public float stopDuration = 0.5f;       // 멈춰있는 시간
    private float stopTimer = 0f;
    private float pauseTimer = 0f;
    private bool isStopping = false;

    [Header("범위 표시 프리팹")]
    public GameObject dashPreviewPrefab;
    public float previewDistanceFromEnemy = 0f;
    public float previewBackOffset = 0f;

    [Header("포션 관련")]
    public GameObject potionPrefab;
    public float potionLifetime = 2f;

    private GameObject dashPreviewInstance;

    void Start()
    {
        spriter = GetComponent<SpriteRenderer>();
        enemyAnimation = GetComponent<EnemyAnimation>();

        if (dashPreviewPrefab != null)
        {
            dashPreviewInstance = Instantiate(dashPreviewPrefab, transform.position, Quaternion.identity);
            dashPreviewInstance.SetActive(false);
        }

        originalSpeed = GameManager.Instance.potionEnemyStats.speed;
        speed = originalSpeed;
    }

    void Update()
    {
        if (!isLive) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        Vector2 dirVec = (player.transform.position - transform.position);
        Vector2 inputVec = dirVec.normalized;

        // 멈춰있는 상태
        if (isStopping)
        {
            pauseTimer += Time.deltaTime;
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Idle);

            if (dashPreviewInstance != null)
            {
                dashPreviewInstance.SetActive(true);

                Vector3 direction = new Vector3(inputVec.x, inputVec.y, 0f).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                dashPreviewInstance.transform.rotation = Quaternion.Euler(0f, 0f, angle);

                Vector3 basePos = transform.position + direction * previewDistanceFromEnemy;
                Vector3 offset = -dashPreviewInstance.transform.up * previewBackOffset;
                dashPreviewInstance.transform.position = basePos + offset;
            }

            if (pauseTimer >= stopDuration)
            {
                isStopping = false;
                pauseTimer = 0f;
                stopTimer = 0f;

                // 포션 생성 (PoolManager 사용)
                if (potionPrefab != null)
                {
                    GameObject potion = PoolManager.Instance.SpawnFromPool(potionPrefab.name, transform.position, Quaternion.identity);

                    if (potion != null)
                    {
                        PotionBehavior pb = potion.GetComponent<PotionBehavior>();
                        if (pb != null)
                            pb.StartLifetime(potionLifetime);
                    }
                }

                if (dashPreviewInstance != null)
                    dashPreviewInstance.SetActive(false);
            }

            return;
        }

        // 멈춤 시작 조건
        stopTimer += Time.deltaTime;
        if (stopTimer >= stopCooldown)
        {
            isStopping = true;
            pauseTimer = 0f;
            // 이동 멈춤 상태이므로 transform 이동도 하지 않음
            return;
        }

        // 추적 이동, transform.Translate 사용
        currentDirection = Vector2.SmoothDamp(currentDirection, inputVec, ref currentVelocity, smoothTime);
        Vector2 nextVec = currentDirection * speed * Time.deltaTime;
        transform.Translate(nextVec);

        // 애니메이션 및 방향 전환
        if (currentDirection.magnitude > 0.01f)
        {
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Move);
            FlipSprite(currentDirection.x);
        }
        else
        {
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Idle);
        }

        if (dashPreviewInstance != null && !isStopping)
            dashPreviewInstance.SetActive(false);
    }

    private void FlipSprite(float directionX)
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (directionX < 0 ? -1 : 1);
        transform.localScale = scale;
    }

    void OnDisable()
    {
        if (dashPreviewInstance != null)
        {
            dashPreviewInstance.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (dashPreviewInstance != null)
        {
            Destroy(dashPreviewInstance);
        }
    }
}
