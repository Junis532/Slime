using DG.Tweening;
using UnityEngine;

public class Enemy : EnemyBase
{
    private bool isLive = true;

    private SpriteRenderer spriter;
    private EnemyAnimation enemyAnimation;

    private Vector2 currentVelocity;
    private Vector2 currentDirection;

    public float smoothTime = 0.1f;

    [Header("회피 관련")]
    public float avoidanceRange = 2f;        // 장애물 감지 범위
    public LayerMask obstacleMask;           // 장애물 레이어 지정

    void Start()
    {
        spriter = GetComponent<SpriteRenderer>();
        enemyAnimation = GetComponent<EnemyAnimation>();

        originalSpeed = GameManager.Instance.enemyStats.speed;
        speed = originalSpeed;
    }

    void Update()
    {
        if (!isLive) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        Vector2 currentPos = transform.position;
        Vector2 dirToPlayer = ((Vector2)player.transform.position - currentPos).normalized;

        // ------------------ 장애물 레이캐스트 검사 ------------------
        RaycastHit2D hit = Physics2D.Raycast(currentPos, dirToPlayer, avoidanceRange, obstacleMask);

        Vector2 avoidanceVector = Vector2.zero;

        if (hit.collider != null)
        {
            // 장애물이 감지됨 → 옆으로 회피 방향 계산
            Vector2 hitNormal = hit.normal; // 장애물 표면 노멀 벡터

            // hitNormal은 장애물에 수직인 방향이므로, 이를 기준으로 옆으로 회피
            // 옆 방향 벡터 = hitNormal과 수직 벡터 중 하나 선택
            Vector2 sideStep = Vector2.Perpendicular(hitNormal);

            // 양쪽 중 적절한 방향 선택 (ex: 오른쪽 방향으로)
            // 필요하면 cross나 dot 써서 방향 바꿀 수도 있음
            avoidanceVector = sideStep.normalized * 1.5f; // 강도 조절

            // Debug용
            Debug.DrawRay(currentPos, sideStep * 2, Color.green);
        }

        // ------------------ 최종 방향 ------------------
        Vector2 finalDir = (dirToPlayer + avoidanceVector).normalized;

        currentDirection = Vector2.SmoothDamp(currentDirection, finalDir, ref currentVelocity, smoothTime);

        Vector2 moveVec = currentDirection * speed * Time.deltaTime;
        transform.Translate(moveVec);

        // 방향 및 애니메이션 처리
        if (currentDirection.magnitude > 0.01f)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (currentDirection.x < 0 ? -1 : 1);
            transform.localScale = scale;

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
                // 죽음 처리
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, avoidanceRange);
        // Raycast 시각화는 Debug.DrawRay()로 확인 가능
    }
}
