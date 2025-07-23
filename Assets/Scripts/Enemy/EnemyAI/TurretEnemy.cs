using DG.Tweening;
using UnityEngine;

public class TurretEnemy : EnemyBase
{
    private bool isLive = true;
    private SpriteRenderer spriter;
    private EnemyAnimation enemyAnimation;

    public float fireRange = 5f;             // 발사 범위
    public float fireCooldown = 1.5f;        // 발사 쿨다운
    private float lastFireTime;

    public GameObject bulletPrefab;
    public float bulletSpeed = 3f;
    public float bulletLifetime = 3f;

    [Header("시각적 범위 표시")]
    public GameObject rangeVisualPrefab;     // 범위를 표시할 프리팹 (예: 투명한 원)
    private GameObject rangeVisualInstance;

    void Start()
    {
        spriter = GetComponent<SpriteRenderer>();
        enemyAnimation = GetComponent<EnemyAnimation>();

        // 시각적 범위 표시 생성
        if (rangeVisualPrefab != null)
        {
            rangeVisualInstance = Instantiate(rangeVisualPrefab, transform.position, Quaternion.identity, transform);
            rangeVisualInstance.transform.localScale = Vector3.one * fireRange * 2f;
        }

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

        // 좌우 반전
        if (toPlayer.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (toPlayer.x < 0 ? -1 : 1);
            transform.localScale = scale;
        }

        if (distance <= fireRange)
        {
            if (Time.time - lastFireTime >= fireCooldown)
            {
                Shoot(toPlayer.normalized);
                lastFireTime = Time.time;
            }

            enemyAnimation.PlayAnimation(EnemyAnimation.State.Idle); //  공격 시 애니메이션
        }
    }


    void Shoot(Vector2 dir)
    {
        GameObject bullet = PoolManager.Instance.SpawnFromPool(bulletPrefab.name, transform.position, Quaternion.identity);

        if (bullet != null)
        {
            BulletBehavior bulletBehavior = bullet.GetComponent<BulletBehavior>();
            if (bulletBehavior == null)
                bulletBehavior = bullet.AddComponent<BulletBehavior>();

            bulletBehavior.Initialize(dir.normalized, bulletSpeed, bulletLifetime);
        }
    }

    private void OnDestroy()
    {
        if (rangeVisualInstance != null)
        {
            Destroy(rangeVisualInstance);
        }

        isLive = false;
    }
}
