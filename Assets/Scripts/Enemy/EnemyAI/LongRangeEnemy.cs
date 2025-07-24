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

    [Header("ȸ�� ����")]
    public float avoidanceRange = 2f;       // ��ֹ� ���� ����
    public LayerMask obstacleMask;          // ��ֹ� ���̾� ����

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

        Vector2 currentPos = transform.position;
        Vector2 toPlayer = (Vector2)player.transform.position - currentPos;
        float distance = toPlayer.magnitude;
        Vector2 dirToPlayer = toPlayer.normalized;

        // ------------------ ��ֹ� ����ĳ��Ʈ �˻� ------------------
        RaycastHit2D hit = Physics2D.Raycast(currentPos, dirToPlayer, avoidanceRange, obstacleMask);

        Vector2 avoidanceVector = Vector2.zero;

        if (hit.collider != null)
        {
            // ��ֹ��� ������ �� ������ ȸ�� ���� ���
            Vector2 hitNormal = hit.normal; // ��ֹ� ǥ�� ��� ����

            // hitNormal�� ������ ����(������)
            Vector2 sideStep = Vector2.Perpendicular(hitNormal);

            // ������ �Ǵ� ���� �������� ����(���⼱ ����������)
            avoidanceVector = sideStep.normalized * 1.5f;

            Debug.DrawRay(currentPos, sideStep * 2, Color.green);
        }

        // ------------------ ���� �̵� ���� ��� ------------------
        Vector2 moveDir;

        if (distance < safeDistance)
        {
            // ������ �������鼭 ����
            moveDir = (-dirToPlayer + avoidanceVector).normalized;

            if (Time.time - lastFireTime >= fireCooldown)
            {
                Shoot(dirToPlayer); // �÷��̾� ������ �Ѿ� �߻�
                lastFireTime = Time.time;
            }
        }
        else
        {
            // �ָ� �÷��̾� ������ �̵� + ȸ��
            moveDir = (dirToPlayer + avoidanceVector).normalized;
        }

        currentDirection = Vector2.SmoothDamp(currentDirection, moveDir, ref currentVelocity, smoothTime);
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
        GameObject bullet = PoolManager.Instance.SpawnFromPool(bulletPrefab.name, transform.position, Quaternion.identity);

        if (bullet != null)
        {
            BulletBehavior bulletBehavior = bullet.GetComponent<BulletBehavior>();
            if (bulletBehavior == null)
                bulletBehavior = bullet.AddComponent<BulletBehavior>();

            bulletBehavior.Initialize(dir.normalized, bulletSpeed, bulletLifetime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isLive) return;

        if (collision.CompareTag("Player"))
        {
            int damage = GameManager.Instance.longRangeEnemyStats.attack;
            GameManager.Instance.playerStats.currentHP -= damage;
            GameManager.Instance.playerDamaged.PlayDamageEffect();

            if (GameManager.Instance.playerStats.currentHP <= 0)
            {
                GameManager.Instance.playerStats.currentHP = 0;
                // ���� ó��
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, avoidanceRange);
        // ����ĳ��Ʈ �ð�ȭ�� Debug.DrawRay()�� Ȯ���ϼ���.
    }
}
