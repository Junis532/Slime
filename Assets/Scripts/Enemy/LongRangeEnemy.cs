using UnityEngine;

public class LongRangeEnemy : MonoBehaviour
{
    private bool isLive = true;
    private SpriteRenderer spriter;
    private EnemyAnimation enemyAnimation;

    private Vector2 currentVelocity;
    private Vector2 currentDirection;

    public float smoothTime = 0.1f;
    public float safeDistance = 3f;         // 이 거리 안에 플레이어가 들어오면 도망감 + 공격 시작

    public GameObject bulletPrefab;         // 발사할 탄환 프리팹
    public float bulletSpeed = 5f;          // 탄환 속도
    public float fireCooldown = 1.5f;       // 발사 쿨다운 시간
    private float lastFireTime;             // 마지막 발사 시점

    public float bulletMaxDistance = 10f;  // 총알 최대 비행 거리

    [Header("죽을 때 드랍할 코인")]
    public GameObject coinPrefab;

    void Start()
    {
        spriter = GetComponent<SpriteRenderer>();
        enemyAnimation = GetComponent<EnemyAnimation>();
    }

    void Update()
    {
        if (!isLive) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        Vector2 toPlayer = player.transform.position - transform.position;
        float distance = toPlayer.magnitude;
        Vector2 inputVec;

        if (distance < safeDistance)
        {
            // 도망 방향
            inputVec = (-toPlayer).normalized;

            // 쿨다운이 지났다면 탄환 발사
            if (Time.time - lastFireTime >= fireCooldown)
            {
                Shoot(toPlayer.normalized);
                lastFireTime = Time.time;
            }
        }
        else
        {
            // 추적 방향
            inputVec = toPlayer.normalized;
        }

        // 방향 보간
        currentDirection = Vector2.SmoothDamp(currentDirection, inputVec, ref currentVelocity, smoothTime);

        // 이동
        Vector2 nextVec = currentDirection * GameManager.Instance.longRangeEnemyStats.speed * Time.deltaTime;
        transform.Translate(nextVec);

        // 좌우 반전 처리
        if (currentDirection.magnitude > 0.01f)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (currentDirection.x < 0 ? -1 : 1);
            transform.localScale = scale;
        }

        // 애니메이션 처리
        if (currentDirection.magnitude > 0.01f)
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Move);
        else
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Idle);
    }


    public void Die()
    {
        if (!isLive) return;

        isLive = false;
        //enemyAnimation.PlayAnimation(EnemyAnimation.State.Die);

        // 코인 생성
        if (coinPrefab != null)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }

        // 적 제거 (사망 애니메이션 시간에 맞게 딜레이)
        Destroy(gameObject);
    }

    void Shoot(Vector2 dir)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        // 총알에 BulletBehavior 컴포넌트 붙이기 (없으면 추가)
        BulletBehavior bulletBehavior = bullet.GetComponent<BulletBehavior>();
        if (bulletBehavior == null)
            bulletBehavior = bullet.AddComponent<BulletBehavior>();

        bulletBehavior.Initialize(dir.normalized, bulletSpeed, bulletMaxDistance);
    }
}


/// <summary>
/// 총알의 움직임과 최대 비행거리 관리 스크립트
/// </summary>
public class BulletBehavior : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float maxDistance;

    private Vector2 startPos;

    private Rigidbody2D rb;

    public void Initialize(Vector2 dir, float spd, float maxDist)
    {
        direction = dir;
        speed = spd;
        maxDistance = maxDist;
    }

    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        rb.linearVelocity = direction * speed;
    }

    void Update()
    {
        float traveled = Vector2.Distance(startPos, transform.position);
        if (traveled >= maxDistance)
        {
            Destroy(gameObject);
        }
    }
}