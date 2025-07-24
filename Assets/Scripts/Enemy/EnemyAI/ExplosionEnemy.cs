using DG.Tweening;
using System.Collections;
using UnityEngine;

public class ExplosionEnemy : EnemyBase
{
    private bool isLive = true;

    private SpriteRenderer spriter;
    private EnemyAnimation enemyAnimation;

    private Vector2 currentVelocity;
    private Vector2 currentDirection;

    public float smoothTime = 0.1f;
    public float explosionRange = 1.5f; // ���� ����
    public GameObject explosionEffectPrefab; // ���� ����Ʈ

    [Header("ȸ�� ����")]
    public float avoidanceRange = 1.5f;        // ��ֹ� ���� ����
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
        Vector2 dirToPlayer = ((Vector2)player.transform.position - currentPos);
        float distanceToPlayer = dirToPlayer.magnitude;

        // ���� ����
        if (distanceToPlayer <= explosionRange)
        {
            Explode(player.transform.position);
            return;
        }

        // ��ֹ� ȸ�� ���
        RaycastHit2D hit = Physics2D.Raycast(currentPos, dirToPlayer.normalized, avoidanceRange, obstacleMask);
        Vector2 avoidanceVector = Vector2.zero;

        if (hit.collider != null)
        {
            Vector2 hitNormal = hit.normal;
            Vector2 sideStep = Vector2.Perpendicular(hitNormal); // ���� ����
            avoidanceVector = sideStep.normalized * 1.5f;

            Debug.DrawRay(currentPos, sideStep * 2f, Color.green); // �����
        }

        // ȸ�� ���Ϳ� �÷��̾� ���� ����
        Vector2 finalDir = (dirToPlayer.normalized + avoidanceVector).normalized;
        currentDirection = Vector2.SmoothDamp(currentDirection, finalDir, ref currentVelocity, smoothTime);
        Vector2 moveVec = currentDirection * speed * Time.deltaTime;
        transform.Translate(moveVec);

        // ���� ���� �� �ִϸ��̼� ó��
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

    private void Explode(Vector3 position)
    {
        if (!isLive) return;
        isLive = false;

        if (explosionEffectPrefab != null)
        {
            GameObject effect = Instantiate(explosionEffectPrefab, position, Quaternion.identity);
            Destroy(effect, 0.3f);
        }

        int damage = GameManager.Instance.enemyStats.attack;
        GameManager.Instance.playerStats.currentHP -= damage;

        if (GameManager.Instance.playerDamaged != null)
            GameManager.Instance.playerDamaged.PlayDamageEffect(); // Null ���� ����

        if (GameManager.Instance.playerStats.currentHP <= 0)
        {
            GameManager.Instance.playerStats.currentHP = 0;
            // �÷��̾� ���� ó��
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isLive) return;

        if (collision.CompareTag("Player"))
        {
            Explode(transform.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, avoidanceRange);
    }
}
