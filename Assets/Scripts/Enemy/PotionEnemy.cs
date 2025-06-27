using UnityEngine;
using System.Collections;
public class PotionEnemy : MonoBehaviour
{
    private bool isLive = true;

    private SpriteRenderer spriter;
    private EnemyAnimation enemyAnimation;
    private Rigidbody2D rb;

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

    private GameObject dashPreviewInstance;

    [Header("���� �� ����� ����")]
    public GameObject coinPrefab;

    void Start()
    {
        spriter = GetComponent<SpriteRenderer>();
        enemyAnimation = GetComponent<EnemyAnimation>();
        rb = GetComponent<Rigidbody2D>();

        if (dashPreviewPrefab != null)
        {
            dashPreviewInstance = Instantiate(dashPreviewPrefab, transform.position, Quaternion.identity);
            dashPreviewInstance.SetActive(false);
        }
    }

    void Update()
    {
        if (!isLive) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        Vector2 dirVec = (player.transform.position - transform.position);
        Vector2 inputVec = dirVec.normalized;

        // �����ִ� ����
        if (isStopping)
        {
            pauseTimer += Time.deltaTime;
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Idle);

            if (dashPreviewInstance != null)
            {
                dashPreviewInstance.SetActive(true);

                Vector3 direction = new Vector3(inputVec.x, inputVec.y, 0f).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                dashPreviewInstance.transform.rotation = Quaternion.Euler(0f, 0f, angle);

                Vector3 basePos = transform.position + direction * previewDistanceFromEnemy;
                Vector3 offset = -dashPreviewInstance.transform.up * previewBackOffset;
                dashPreviewInstance.transform.position = basePos + offset;
            }

            if (pauseTimer >= stopDuration)
            {
                isStopping = false;
                pauseTimer = 0f;
                stopTimer = 0f;

                // ���� ����
                if (potionPrefab != null)
                {
                    GameObject potion = Instantiate(potionPrefab, transform.position, Quaternion.identity);
                    Destroy(potion, potionLifetime);
                }

                // �̵� ��� ����
                if (rb != null)
                {
                    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
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

            // �̵� ���
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            }

            return;
        }

        // ���� �̵�
        currentDirection = Vector2.SmoothDamp(currentDirection, inputVec, ref currentVelocity, smoothTime);
        Vector2 nextVec = currentDirection * GameManager.Instance.potionEnemyStats.speed * Time.deltaTime;
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
