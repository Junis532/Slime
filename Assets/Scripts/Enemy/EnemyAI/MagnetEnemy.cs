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

    void Start()
    {
        spriter = GetComponent<SpriteRenderer>();
        enemyAnimation = GetComponent<EnemyAnimation>();

        originalSpeed = followSpeed;
        speed = originalSpeed;

        // ���� ���� ǥ��
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

        Vector2 toPlayer = player.transform.position - transform.position;
        float distance = toPlayer.magnitude;

        // �¿� ����
        if (toPlayer.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (toPlayer.x < 0 ? -1 : 1);
            transform.localScale = scale;
        }

        // �������������������������� �÷��̾ ������ ������ ��������������������������
        if (distance <= detectionRange)
        {
            Vector3 pullDir = (transform.position - player.transform.position).normalized;
            player.transform.position += pullDir * pullForce * Time.deltaTime;
        }

        // �������������������������� ���� �׻� ���� ��������������������������
        Vector2 dirVec = toPlayer.normalized;
        currentDirection = Vector2.SmoothDamp(currentDirection, dirVec, ref currentVelocity, smoothTime);
        Vector2 nextVec = currentDirection * followSpeed * Time.deltaTime;
        transform.Translate(nextVec);

        // �ִϸ��̼�
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
