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
    public float safeDistance = 3f;         // �÷��̾ �� �Ÿ� �ȿ� ���� ���� + ���� ����

    public GameObject bulletPrefab;         // �߻��� źȯ ������
    public float bulletSpeed = 3f;          // źȯ �ӵ�
    public float fireCooldown = 1.5f;       // �߻� ��ٿ�
    private float lastFireTime;             // ������ �߻� ����

    public float bulletLifetime = 3f;       // �Ѿ� ���� �ð� (��)

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
            // �÷��̾ ������ �������鼭 ����
            inputVec = (-toPlayer).normalized;

            if (Time.time - lastFireTime >= fireCooldown)
            {
                Shoot(toPlayer.normalized);
                lastFireTime = Time.time;
            }
        }
        else
        {
            // �ָ� �÷��̾� ������ �̵�
            inputVec = toPlayer.normalized;
        }

        // ���� ���� �̵�, �ӵ��� ���� speed ���
        currentDirection = Vector2.SmoothDamp(currentDirection, inputVec, ref currentVelocity, smoothTime);
        Vector2 nextVec = currentDirection * speed * Time.deltaTime;
        transform.Translate(nextVec);

        // �¿� ����
        if (currentDirection.magnitude > 0.01f)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (currentDirection.x < 0 ? -1 : 1);
            transform.localScale = scale;
        }

        // �ִϸ��̼� ó��
        if (currentDirection.magnitude > 0.01f)
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Move);
        else
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Idle);
    }

    void Shoot(Vector2 dir)
    {
        // PoolManager�� �Ѿ� ��ȯ
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

