using UnityEngine;

public class SquareEnemy : EnemyBase
{
    private bool isLive = true;

    private SpriteRenderer spriter;
    private EnemyAnimation enemyAnimation;

    [Header("��� ����")]
    public Vector2 pathCenter = Vector2.zero;
    public float pathWidth = 20f;
    public float pathHeight = 12f;

    private Vector2[] waypoints;
    private int currentWaypointIndex = 0;

    private Vector2 currentVelocity;
    private Vector2 currentDirection;

    public float smoothTime = 0.1f;

    [Header("ȸ�� ����")]
    public float avoidanceRange = 2f;
    public LayerMask obstacleMask;

    void Start()
    {
        spriter = GetComponent<SpriteRenderer>();
        enemyAnimation = GetComponent<EnemyAnimation>();

        originalSpeed = GameManager.Instance.enemyStats.speed;
        speed = originalSpeed;

        GenerateWaypoints();
        transform.position = waypoints[0];
        currentWaypointIndex = 1;
    }

    void GenerateWaypoints()
    {
        float halfWidth = pathWidth / 2f;
        float halfHeight = pathHeight / 2f;

        waypoints = new Vector2[]
        {
            pathCenter + new Vector2(-halfWidth,  halfHeight),
            pathCenter + new Vector2( halfWidth,  halfHeight),
            pathCenter + new Vector2( halfWidth, -halfHeight),
            pathCenter + new Vector2(-halfWidth, -halfHeight)
        };
    }

    void Update()
    {
        if (!isLive) return;

        Vector2 currentPos = transform.position;
        Vector2 target = waypoints[currentWaypointIndex];
        Vector2 dirToTarget = (target - currentPos).normalized;

        // ��ֹ� ȸ�� �˻�
        RaycastHit2D hit = Physics2D.Raycast(currentPos, dirToTarget, avoidanceRange, obstacleMask);

        Vector2 avoidanceVector = Vector2.zero;
        if (hit.collider != null)
        {
            Vector2 hitNormal = hit.normal;
            Vector2 sideStep = Vector2.Perpendicular(hitNormal).normalized;
            avoidanceVector = sideStep * 1.5f;
            Debug.DrawRay(currentPos, sideStep * 2, Color.green);
        }

        // ���� ����
        Vector2 finalDir = (dirToTarget + avoidanceVector).normalized;
        currentDirection = Vector2.SmoothDamp(currentDirection, finalDir, ref currentVelocity, smoothTime);
        Vector2 moveVec = currentDirection * speed * Time.deltaTime;
        transform.Translate(moveVec);

        if (Vector2.Distance(transform.position, target) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }

        // ���� ����
        if (currentDirection.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (currentDirection.x < 0 ? -1 : 1);
            transform.localScale = scale;
        }

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
            GameManager.Instance.playerDamaged.PlayDamageEffect();

            if (GameManager.Instance.playerStats.currentHP <= 0)
            {
                GameManager.Instance.playerStats.currentHP = 0;
                // �÷��̾� ��� ó��
            }
        }
    }

    private void OnDestroy()
    {
        isLive = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, avoidanceRange);

        // �簢�� ��� ǥ��
        Vector2 p1 = pathCenter + new Vector2(-pathWidth / 2f, pathHeight / 2f);
        Vector2 p2 = pathCenter + new Vector2(pathWidth / 2f, pathHeight / 2f);
        Vector2 p3 = pathCenter + new Vector2(pathWidth / 2f, -pathHeight / 2f);
        Vector2 p4 = pathCenter + new Vector2(-pathWidth / 2f, -pathHeight / 2f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);
    }
}
