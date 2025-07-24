using UnityEngine;

public class DetectEnemy : EnemyBase
{
    private bool isLive = true;

    private SpriteRenderer spriter;
    private EnemyAnimation enemyAnimation;

    private Vector2 currentVelocity;
    private Vector2 currentDirection;
    private Vector2 randomDirection;

    public float smoothTime = 0.1f;
    public float detectionRange = 5f;
    private bool hasDetectedPlayer = false;

    [Header("시각적 범위 표시")]
    public GameObject rangeVisualPrefab;
    private GameObject rangeVisualInstance;

    private float randomMoveTimer = 0f;
    public float randomChangeInterval = 2f;

    private readonly float minX = -10f;
    private readonly float maxX = 10f;
    private readonly float minY = -6f;
    private readonly float maxY = 6f;

    [Header("회피 관련")]
    public float avoidanceRange = 2f;
    public LayerMask obstacleMask;

    void Start()
    {
        spriter = GetComponent<SpriteRenderer>();
        enemyAnimation = GetComponent<EnemyAnimation>();

        originalSpeed = GameManager.Instance.enemyStats.speed;
        speed = originalSpeed;

        if (rangeVisualPrefab != null)
        {
            rangeVisualInstance = Instantiate(rangeVisualPrefab, transform.position, Quaternion.identity, transform);
            rangeVisualInstance.transform.localScale = Vector3.one * detectionRange * 2f;
        }

        PickRandomDirection();
    }

    void Update()
    {
        if (!isLive) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        Vector2 toPlayer = player.transform.position - transform.position;
        float distance = toPlayer.magnitude;

        if (toPlayer.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (toPlayer.x < 0 ? -1 : 1);
            transform.localScale = scale;
        }

        if (!hasDetectedPlayer && distance <= detectionRange)
        {
            hasDetectedPlayer = true;

            if (rangeVisualInstance != null)
                Destroy(rangeVisualInstance);
        }

        if (hasDetectedPlayer)
        {
            TrackPlayerWithAvoidance(player);
        }
        else
        {
            RandomMove();
        }
    }

    /// <summary>
    /// 플레이어 추적 + 장애물 회피
    /// </summary>
    private void TrackPlayerWithAvoidance(GameObject player)
    {
        Vector2 currentPos = transform.position;
        Vector2 dirToPlayer = ((Vector2)player.transform.position - currentPos).normalized;

        // 장애물 감지
        RaycastHit2D hit = Physics2D.Raycast(currentPos, dirToPlayer, avoidanceRange, obstacleMask);
        Vector2 avoidanceVector = Vector2.zero;

        if (hit.collider != null)
        {
            Vector2 hitNormal = hit.normal;
            Vector2 sideStep = Vector2.Perpendicular(hitNormal); // 옆으로 피해감

            avoidanceVector = sideStep.normalized * 1.5f;

            // Debug
            Debug.DrawRay(currentPos, sideStep * 2, Color.green);
        }

        // 최종 방향
        Vector2 finalDir = (dirToPlayer + avoidanceVector).normalized;

        currentDirection = Vector2.SmoothDamp(currentDirection, finalDir, ref currentVelocity, smoothTime);
        Vector2 moveVec = currentDirection * speed * Time.deltaTime;

        transform.Translate(moveVec);

        // 애니메이션
        if (moveVec.magnitude > 0.01f)
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Move);
        else
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Idle);
    }

    private void RandomMove()
    {
        randomMoveTimer += Time.deltaTime;

        if (randomMoveTimer >= randomChangeInterval)
        {
            PickRandomDirection();
            randomMoveTimer = 0f;
        }

        Vector2 moveVec = randomDirection.normalized * speed * Time.deltaTime;
        Vector3 newPos = transform.position + (Vector3)moveVec;

        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

        bool hitBoundary = false;
        if (newPos.x == minX || newPos.x == maxX || newPos.y == minY || newPos.y == maxY)
            hitBoundary = true;

        transform.position = newPos;

        if (hitBoundary)
        {
            PickRandomDirection();
            randomMoveTimer = 0f;
        }

        if (moveVec.magnitude > 0.01f)
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Move);
        else
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Idle);
    }

    private void PickRandomDirection()
    {
        Vector2[] directions = {
            Vector2.left,
            Vector2.right,
            Vector2.up,
            Vector2.down
        };
        randomDirection = directions[Random.Range(0, directions.Length)];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isLive) return;

        if (collision.CompareTag("Player"))
        {
            int damage = GameManager.Instance.enemyStats.attack;
            GameManager.Instance.playerStats.currentHP -= damage;
            GameManager.Instance.playerDamaged.PlayDamageEffect();

            if (GameManager.Instance.playerStats.currentHP <= 0)
                GameManager.Instance.playerStats.currentHP = 0;
        }
    }

    private void OnDestroy()
    {
        isLive = false;

        if (rangeVisualInstance != null)
            Destroy(rangeVisualInstance);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, avoidanceRange);
    }
}
