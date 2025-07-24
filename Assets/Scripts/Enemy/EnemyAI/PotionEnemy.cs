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

    [Header("회피 관련")]
    public float avoidanceRange = 2f;       // 장애물 감지 범위
    public LayerMask obstacleMask;          // 장애물 레이어 지정

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

        Vector2 currentPos = transform.position;
        Vector2 toPlayer = (Vector2)player.transform.position - currentPos;
        Vector2 inputVec = toPlayer.normalized;

        // 장애물 감지 레이캐스트
        RaycastHit2D hit = Physics2D.Raycast(currentPos, inputVec, avoidanceRange, obstacleMask);

        Vector2 avoidanceVector = Vector2.zero;

        if (hit.collider != null)
        {
            Vector2 hitNormal = hit.normal;
            Vector2 sideStep = Vector2.Perpendicular(hitNormal);

            // 오른쪽 방향으로 회피
            avoidanceVector = sideStep.normalized * 1.5f;

            Debug.DrawRay(currentPos, sideStep * 2, Color.green);
        }

        // 멈춰있는 상태
        if (isStopping)
        {
            pauseTimer += Time.deltaTime;
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Idle);

            if (dashPreviewInstance != null)
            {
                // 범위 표시 각도 조정
                Vector3 direction = (inputVec + avoidanceVector).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                dashPreviewInstance.transform.rotation = Quaternion.Euler(0f, 0f, angle);

                Vector3 basePos = transform.position + direction * previewDistanceFromEnemy;
                Vector3 offset = -dashPreviewInstance.transform.up * previewBackOffset;
                dashPreviewInstance.transform.position = basePos + offset;
                dashPreviewInstance.SetActive(true);
            }

            if (pauseTimer >= stopDuration)
            {
                isStopping = false;
                pauseTimer = 0f;
                stopTimer = 0f;

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
            return;
        }

        // 장애물 회피를 포함한 최종 이동 방향
        Vector2 finalDir = (inputVec + avoidanceVector).normalized;

        currentDirection = Vector2.SmoothDamp(currentDirection, finalDir, ref currentVelocity, smoothTime);
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
