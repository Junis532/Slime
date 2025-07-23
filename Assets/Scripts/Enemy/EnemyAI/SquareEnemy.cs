using UnityEngine;

public class SquareEnemy : EnemyBase
{
    private bool isLive = true;

    private SpriteRenderer spriter;
    private EnemyAnimation enemyAnimation;

    [Header("직사각형 경로 설정")]
    private Vector2[] waypoints;
    private int currentWaypointIndex = 0;

    void Start()
    {
        spriter = GetComponent<SpriteRenderer>();
        enemyAnimation = GetComponent<EnemyAnimation>();

        originalSpeed = GameManager.Instance.enemyStats.speed;
        speed = originalSpeed;

        // 꼭짓점 설정 (시계 방향 순서)
        waypoints = new Vector2[]
        {
            new Vector2(-10,  6),
            new Vector2( 10,  6),
            new Vector2( 10, -6),
            new Vector2(-10, -6)
        };

        // 시작 위치 설정
        transform.position = waypoints[0];
        currentWaypointIndex = 1;
    }

    void Update()
    {
        if (!isLive) return;

        Vector2 target = waypoints[currentWaypointIndex];
        Vector2 moveDir = (target - (Vector2)transform.position).normalized;

        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // 방향 반전 (x축 기준)
        if (moveDir.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (moveDir.x < 0 ? -1 : 1);
            transform.localScale = scale;
        }

        enemyAnimation.PlayAnimation(EnemyAnimation.State.Move);

        // 다음 꼭짓점으로 이동
        if (Vector2.Distance(transform.position, target) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
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
                // 플레이어 사망 처리 필요 시 여기에
            }
        }
    }

    private void OnDestroy()
    {
        isLive = false;
    }
}
