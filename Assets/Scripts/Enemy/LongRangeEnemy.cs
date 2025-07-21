using DG.Tweening;
using UnityEngine;

public class LongRangeEnemy : EnemyBase
{
    private bool isLive = true;
    private SpriteRenderer spriter;
    private EnemyAnimation enemyAnimation;

    private Vector2 currentVelocity;
    private Vector2 currentDirection;

    public float smoothTime = 0.1f;
    public float safeDistance = 3f;         // 플레이어가 이 거리 안에 오면 도망 + 공격 시작

    public GameObject bulletPrefab;         // 발사할 탄환 프리팹
    public float bulletSpeed = 3f;          // 탄환 속도
    public float fireCooldown = 1.5f;       // 발사 쿨다운
    private float lastFireTime;             // 마지막 발사 시점

    public float bulletLifetime = 3f;       // 총알 생존 시간 (초)

    void Start()
    {
        spriter = GetComponent<SpriteRenderer>();
        enemyAnimation = GetComponent<EnemyAnimation>();

        originalSpeed = GameManager.Instance.longRangeEnemyStats.speed;
        speed = originalSpeed;
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
            // 플레이어가 가까우면 도망가면서 공격
            inputVec = (-toPlayer).normalized;

            if (Time.time - lastFireTime >= fireCooldown)
            {
                Shoot(toPlayer.normalized);
                lastFireTime = Time.time;
            }
        }
        else
        {
            // 멀면 플레이어 쪽으로 이동
            inputVec = toPlayer.normalized;
        }

        // 방향 보간 이동, 속도는 개별 speed 사용
        currentDirection = Vector2.SmoothDamp(currentDirection, inputVec, ref currentVelocity, smoothTime);
        Vector2 nextVec = currentDirection * speed * Time.deltaTime;
        transform.Translate(nextVec);

        // 좌우 반전
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

    void Shoot(Vector2 dir)
    {
        // PoolManager로 총알 소환
        GameObject bullet = PoolManager.Instance.SpawnFromPool(bulletPrefab.name, transform.position, Quaternion.identity);

        if (bullet != null)
        {
            BulletBehavior bulletBehavior = bullet.GetComponent<BulletBehavior>();
            if (bulletBehavior == null)
                bulletBehavior = bullet.AddComponent<BulletBehavior>();

            bulletBehavior.Initialize(dir.normalized, bulletSpeed, bulletLifetime);
        }
    }
}

/// <summary>
/// 총알 움직임과 시간 기반 소멸 관리 - Pool 적용
/// </summary>
public class BulletBehavior : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float lifeTime;
    private float timer = 0f;

    // 총알 세팅
    public void Initialize(Vector2 dir, float spd, float lifetime)
    {
        direction = dir;
        speed = spd;
        lifeTime = lifetime;
        timer = 0f;                // 풀 재사용 대비!
    }

    void OnEnable()
    {
        timer = 0f;   // 풀에서 재활성화 시 타이머 초기화
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            PoolManager.Instance.ReturnToPool(gameObject); // Destroy -> ReturnToPool
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Wall"))
        {
            PoolManager.Instance.ReturnToPool(gameObject); // Destroy -> ReturnToPool
        }
    }
}
