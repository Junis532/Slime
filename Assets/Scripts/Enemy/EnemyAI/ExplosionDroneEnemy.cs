using DG.Tweening;
using System.Collections;
using UnityEngine;

public class ExplosionDronEnemy : EnemyBase
{
    private bool isLive = true;

    private SpriteRenderer spriter;
    private EnemyAnimation enemyAnimation;

    private Vector2 currentVelocity;
    private Vector2 currentDirection;

    public float smoothTime = 0.1f;
    public float explosionRange = 1.5f; // ���� ����
    public GameObject explosionEffectPrefab; // ���� ����Ʈ

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

        Vector2 dirVec = (player.transform.position - transform.position);
        float distanceToPlayer = dirVec.magnitude;

        if (distanceToPlayer <= explosionRange)
        {
            Explode(player.transform.position);
            return;
        }

        Vector2 inputVec = dirVec.normalized;
        currentDirection = Vector2.SmoothDamp(currentDirection, inputVec, ref currentVelocity, smoothTime);
        Vector2 nextVec = currentDirection * speed * Time.deltaTime;
        transform.Translate(nextVec);

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

        // ���� ����Ʈ ���� �� 0.3�� �� ����
        if (explosionEffectPrefab != null)
        {
            GameObject effect = Instantiate(explosionEffectPrefab, position, Quaternion.identity);
            Destroy(effect, 0.3f);
        }

        // �÷��̾� ������
        int damage = GameManager.Instance.enemyStats.attack;
        GameManager.Instance.playerStats.currentHP -= damage;
        GameManager.Instance.playerDamaged.PlayDamageEffect();

        if (GameManager.Instance.playerStats.currentHP <= 0)
        {
            GameManager.Instance.playerStats.currentHP = 0;
            // �÷��̾� ���� ó�� ����
        }

        Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �÷��̾�� �浹�ص� �ٷ� ������ �� ���� (�ɼ�)
        if (!isLive) return;

        if (collision.CompareTag("Player"))
        {
            Explode(transform.position);
        }
    }
}
