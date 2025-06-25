using UnityEngine;

public class LongRangeEnemy : MonoBehaviour
{
    private bool isLive = true;
    private SpriteRenderer spriter;
    private EnemyAnimation enemyAnimation;

    private Vector2 currentVelocity;
    private Vector2 currentDirection;

    public float smoothTime = 0.1f;
    public float safeDistance = 3f;         // �� �Ÿ� �ȿ� �÷��̾ ������ ������ + ���� ����

    public GameObject bulletPrefab;         // �߻��� źȯ ������
    public float bulletSpeed = 5f;          // źȯ �ӵ�
    public float fireCooldown = 1.5f;       // �߻� ��ٿ� �ð�
    private float lastFireTime;             // ������ �߻� ����

    public float bulletMaxDistance = 10f;  // �Ѿ� �ִ� ���� �Ÿ�

    [Header("���� �� ����� ����")]
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
            // ���� ����
            inputVec = (-toPlayer).normalized;

            // ��ٿ��� �����ٸ� źȯ �߻�
            if (Time.time - lastFireTime >= fireCooldown)
            {
                Shoot(toPlayer.normalized);
                lastFireTime = Time.time;
            }
        }
        else
        {
            // ���� ����
            inputVec = toPlayer.normalized;
        }

        // ���� ����
        currentDirection = Vector2.SmoothDamp(currentDirection, inputVec, ref currentVelocity, smoothTime);

        // �̵�
        Vector2 nextVec = currentDirection * GameManager.Instance.longRangeEnemyStats.speed * Time.deltaTime;
        transform.Translate(nextVec);

        // �¿� ���� ó��
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


    public void Die()
    {
        if (!isLive) return;

        isLive = false;
        //enemyAnimation.PlayAnimation(EnemyAnimation.State.Die);

        // ���� ����
        if (coinPrefab != null)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }

        // �� ���� (��� �ִϸ��̼� �ð��� �°� ������)
        Destroy(gameObject);
    }

    void Shoot(Vector2 dir)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        // �Ѿ˿� BulletBehavior ������Ʈ ���̱� (������ �߰�)
        BulletBehavior bulletBehavior = bullet.GetComponent<BulletBehavior>();
        if (bulletBehavior == null)
            bulletBehavior = bullet.AddComponent<BulletBehavior>();

        bulletBehavior.Initialize(dir.normalized, bulletSpeed, bulletMaxDistance);
    }
}


/// <summary>
/// �Ѿ��� �����Ӱ� �ִ� ����Ÿ� ���� ��ũ��Ʈ
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