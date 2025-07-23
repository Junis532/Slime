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
            Vector2 dirVec = toPlayer.normalized;
            currentDirection = Vector2.SmoothDamp(currentDirection, dirVec, ref currentVelocity, smoothTime);
            Vector2 nextVec = currentDirection * speed * Time.deltaTime;
            transform.Translate(nextVec);

            enemyAnimation.PlayAnimation(EnemyAnimation.State.Move);
        }
        else
        {
            RandomMove();
        }
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

        // 범위 내 위치 보정
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

        bool hitBoundary = false;

        // 경계 감지 후 방향 변경
        if (newPos.x == minX || newPos.x == maxX)
            hitBoundary = true;
        if (newPos.y == minY || newPos.y == maxY)
            hitBoundary = true;

        transform.position = newPos;

        if (hitBoundary)
        {
            PickRandomDirection();
            randomMoveTimer = 0f; // 방향 바뀌었으니 타이머 초기화
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
            {
                GameManager.Instance.playerStats.currentHP = 0;
                // 사망 처리 가능
            }
        }
    }

    private void OnDestroy()
    {
        isLive = false;

        if (rangeVisualInstance != null)
            Destroy(rangeVisualInstance);
    }
}
