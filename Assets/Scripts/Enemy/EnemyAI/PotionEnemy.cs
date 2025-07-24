using DG.Tweening;
using UnityEngine;

public class PotionEnemy : EnemyBase
{
    private bool isLive = true;

    private SpriteRenderer spriter;
    private EnemyAnimation enemyAnimation;

    private Vector2 currentVelocity;
    private Vector2 currentDirection;

    public float smoothTime = 0.1f;

    public float stopCooldown = 3f;         // ���ߴ� �ֱ�
    public float stopDuration = 0.5f;       // �����ִ� �ð�
    private float stopTimer = 0f;
    private float pauseTimer = 0f;
    private bool isStopping = false;

    [Header("���� ǥ�� ������")]
    public GameObject dashPreviewPrefab;
    public float previewDistanceFromEnemy = 0f;
    public float previewBackOffset = 0f;

    [Header("���� ����")]
    public GameObject potionPrefab;
    public float potionLifetime = 2f;

    [Header("ȸ�� ����")]
    public float avoidanceRange = 2f;       // ��ֹ� ���� ����
    public LayerMask obstacleMask;          // ��ֹ� ���̾� ����

    private GameObject dashPreviewInstance;

    void Start()
    {
        spriter = GetComponent<SpriteRenderer>();
        enemyAnimation = GetComponent<EnemyAnimation>();

        if (dashPreviewPrefab != null)
        {
            dashPreviewInstance = Instantiate(dashPreviewPrefab, transform.position, Quaternion.identity);
            dashPreviewInstance.SetActive(false);
        }

        originalSpeed = GameManager.Instance.potionEnemyStats.speed;
        speed = originalSpeed;
    }

    void Update()
    {
        if (!isLive) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        Vector2 currentPos = transform.position;
        Vector2 toPlayer = (Vector2)player.transform.position - currentPos;
        Vector2 inputVec = toPlayer.normalized;

        // ��ֹ� ���� ����ĳ��Ʈ
        RaycastHit2D hit = Physics2D.Raycast(currentPos, inputVec, avoidanceRange, obstacleMask);

        Vector2 avoidanceVector = Vector2.zero;

        if (hit.collider != null)
        {
            Vector2 hitNormal = hit.normal;
            Vector2 sideStep = Vector2.Perpendicular(hitNormal);

            // ������ �������� ȸ��
            avoidanceVector = sideStep.normalized * 1.5f;

            Debug.DrawRay(currentPos, sideStep * 2, Color.green);
        }

        // �����ִ� ����
        if (isStopping)
        {
            pauseTimer += Time.deltaTime;
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Idle);

            if (dashPreviewInstance != null)
            {
                // ���� ǥ�� ���� ����
                Vector3 direction = (inputVec + avoidanceVector).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                dashPreviewInstance.transform.rotation = Quaternion.Euler(0f, 0f, angle);

                Vector3 basePos = transform.position + direction * previewDistanceFromEnemy;
                Vector3 offset = -dashPreviewInstance.transform.up * previewBackOffset;
                dashPreviewInstance.transform.position = basePos + offset;
                dashPreviewInstance.SetActive(true);
            }

            if (pauseTimer >= stopDuration)
            {
                isStopping = false;
                pauseTimer = 0f;
                stopTimer = 0f;

                if (potionPrefab != null)
                {
                    GameObject potion = PoolManager.Instance.SpawnFromPool(potionPrefab.name, transform.position, Quaternion.identity);
                    if (potion != null)
                    {
                        PotionBehavior pb = potion.GetComponent<PotionBehavior>();
                        if (pb != null)
                            pb.StartLifetime(potionLifetime);
                    }
                }

                if (dashPreviewInstance != null)
                    dashPreviewInstance.SetActive(false);
            }

            return;
        }

        // ���� ���� ����
        stopTimer += Time.deltaTime;
        if (stopTimer >= stopCooldown)
        {
            isStopping = true;
            pauseTimer = 0f;
            return;
        }

        // ��ֹ� ȸ�Ǹ� ������ ���� �̵� ����
        Vector2 finalDir = (inputVec + avoidanceVector).normalized;

        currentDirection = Vector2.SmoothDamp(currentDirection, finalDir, ref currentVelocity, smoothTime);
        Vector2 nextVec = currentDirection * speed * Time.deltaTime;
        transform.Translate(nextVec);

        // �ִϸ��̼� �� ���� ��ȯ
        if (currentDirection.magnitude > 0.01f)
        {
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Move);
            FlipSprite(currentDirection.x);
        }
        else
        {
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Idle);
        }

        if (dashPreviewInstance != null && !isStopping)
            dashPreviewInstance.SetActive(false);
    }

    private void FlipSprite(float directionX)
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (directionX < 0 ? -1 : 1);
        transform.localScale = scale;
    }

    void OnDisable()
    {
        if (dashPreviewInstance != null)
        {
            dashPreviewInstance.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (dashPreviewInstance != null)
        {
            Destroy(dashPreviewInstance);
        }
    }
}
