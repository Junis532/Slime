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

    [Header("ȸ�� ����")]
    public float avoidanceRange = 2f;        // ��ֹ� ���� ����
    public LayerMask obstacleMask;           // ��ֹ� ���̾� ����

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

        // ------------------ ��ֹ� ����ĳ��Ʈ �˻� ------------------
        RaycastHit2D hit = Physics2D.Raycast(currentPos, dirToPlayer, avoidanceRange, obstacleMask);

        Vector2 avoidanceVector = Vector2.zero;

        if (hit.collider != null)
        {
            // ��ֹ��� ������ �� ������ ȸ�� ���� ���
            Vector2 hitNormal = hit.normal; // ��ֹ� ǥ�� ��� ����

            // hitNormal�� ��ֹ��� ������ �����̹Ƿ�, �̸� �������� ������ ȸ��
            // �� ���� ���� = hitNormal�� ���� ���� �� �ϳ� ����
            Vector2 sideStep = Vector2.Perpendicular(hitNormal);

            // ���� �� ������ ���� ���� (ex: ������ ��������)
            // �ʿ��ϸ� cross�� dot �Ἥ ���� �ٲ� ���� ����
            avoidanceVector = sideStep.normalized * 1.5f; // ���� ����

            // Debug��
            Debug.DrawRay(currentPos, sideStep * 2, Color.green);
        }

        // ------------------ ���� ���� ------------------
        Vector2 finalDir = (dirToPlayer + avoidanceVector).normalized;

        currentDirection = Vector2.SmoothDamp(currentDirection, finalDir, ref currentVelocity, smoothTime);

        Vector2 moveVec = currentDirection * speed * Time.deltaTime;
        transform.Translate(moveVec);

        // ���� �� �ִϸ��̼� ó��
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
                // ���� ó��
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, avoidanceRange);
        // Raycast �ð�ȭ�� Debug.DrawRay()�� Ȯ�� ����
    }
}
