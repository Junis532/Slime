using UnityEngine;

public class MagnetEnemy : EnemyBase
{
    private bool isLive = true;

    private SpriteRenderer spriter;
    private EnemyAnimation enemyAnimation;

    private Vector2 currentVelocity;
    private Vector2 currentDirection;

    public float smoothTime = 0.1f;
    public float detectionRange = 5f;

    [Header("시각적 범위 표시")]
    public GameObject rangeVisualPrefab;
    private GameObject rangeVisualInstance;

    [Header("속도 설정")]
    public float followSpeed = 2f;    // 적이 플레이어를 따라가는 속도
    public float pullForce = 1.5f;    // 플레이어를 끌어당기는 힘

    void Start()
    {
        spriter = GetComponent<SpriteRenderer>();
        enemyAnimation = GetComponent<EnemyAnimation>();

        originalSpeed = followSpeed;
        speed = originalSpeed;

        // 감지 범위 표시
        if (rangeVisualPrefab != null)
        {
            rangeVisualInstance = Instantiate(rangeVisualPrefab, transform.position, Quaternion.identity, transform);
            rangeVisualInstance.transform.localScale = Vector3.one * detectionRange * 2f;
        }
    }

    void Update()
    {
        if (!isLive) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        Vector2 toPlayer = player.transform.position - transform.position;
        float distance = toPlayer.magnitude;

        // 좌우 반전
        if (toPlayer.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (toPlayer.x < 0 ? -1 : 1);
            transform.localScale = scale;
        }

        // ───────────── 플레이어를 강제로 끌어당김 ─────────────
        if (distance <= detectionRange)
        {
            Vector3 pullDir = (transform.position - player.transform.position).normalized;
            player.transform.position += pullDir * pullForce * Time.deltaTime;
        }

        // ───────────── 적은 항상 따라감 ─────────────
        Vector2 dirVec = toPlayer.normalized;
        currentDirection = Vector2.SmoothDamp(currentDirection, dirVec, ref currentVelocity, smoothTime);
        Vector2 nextVec = currentDirection * followSpeed * Time.deltaTime;
        transform.Translate(nextVec);

        // 애니메이션
        if (currentDirection.magnitude > 0.01f)
        {
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Move);
        }
        else
        {
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Idle);
        }
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
                // 사망 처리
            }
        }
    }

    private void OnDestroy()
    {
        isLive = false;

        if (rangeVisualInstance != null)
        {
            Destroy(rangeVisualInstance);
        }
    }
}
