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

    [Header("�ð��� ���� ǥ��")]
    public GameObject rangeVisualPrefab;
    private GameObject rangeVisualInstance;

    [Header("�ӵ� ����")]
    public float followSpeed = 2f;    // ���� �÷��̾ ���󰡴� �ӵ�
    public float pullForce = 1.5f;    // �÷��̾ ������� ��

    [Header("ȸ�� ����")]
    public float avoidanceRange = 2f;        // ��ֹ� ���� ����
    public LayerMask obstacleMask;           // ��ֹ� ���̾� ����

    void Start()
    {
        spriter = GetComponent<SpriteRenderer>();
        enemyAnimation = GetComponent<EnemyAnimation>();

        originalSpeed = followSpeed;
        speed = originalSpeed;

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

        Vector2 currentPos = transform.position;
        Vector2 dirToPlayer = ((Vector2)player.transform.position - currentPos);
        float distance = dirToPlayer.magnitude;

        // �¿� ����
        if (dirToPlayer.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (dirToPlayer.x < 0 ? -1 : 1);
            transform.localScale = scale;
        }

        // ��ֹ� ȸ�� ����ĳ��Ʈ �˻�
        Vector2 dirNormalized = dirToPlayer.normalized;
        RaycastHit2D hit = Physics2D.Raycast(currentPos, dirNormalized, avoidanceRange, obstacleMask);

        Vector2 avoidanceVector = Vector2.zero;

        if (hit.collider != null)
        {
            Vector2 hitNormal = hit.normal;
            Vector2 sideStep = Vector2.Perpendicular(hitNormal);

            avoidanceVector = sideStep.normalized * 1.5f; // ���� ����
            Debug.DrawRay(currentPos, sideStep * 2f, Color.green);
        }

        // ���� ���� (�÷��̾� ���� + ȸ�� ����)
        Vector2 finalDir = (dirNormalized + avoidanceVector).normalized;

        // �÷��̾ ������� �� ���� (���� ���� ��)
        if (distance <= detectionRange)
        {
            Vector3 pullDir = (transform.position - player.transform.position).normalized;
            player.transform.position += pullDir * pullForce * Time.deltaTime;
        }

        // �� ������ (ȸ�� ����)
        currentDirection = Vector2.SmoothDamp(currentDirection, finalDir, ref currentVelocity, smoothTime);
        Vector2 moveVec = currentDirection * followSpeed * Time.deltaTime;
        transform.Translate(moveVec);

        // �ִϸ��̼� ó��
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

            if (GameManager.Instance.playerDamaged != null)
                GameManager.Instance.playerDamaged.PlayDamageEffect();

            if (GameManager.Instance.playerStats.currentHP <= 0)
            {
                GameManager.Instance.playerStats.currentHP = 0;
                // ��� ó��
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
