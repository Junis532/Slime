using UnityEngine;

/// <summary>
/// �Ѿ� �����Ӱ� �ð� ��� �Ҹ� ���� - Pool ����
/// </summary>
public class BulletBehavior : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float lifeTime;
    private float timer = 0f;

    // �Ѿ� ����
    public void Initialize(Vector2 dir, float spd, float lifetime)
    {
        direction = dir;
        speed = spd;
        lifeTime = lifetime;
        timer = 0f;                // Ǯ ���� ���!
    }

    void OnEnable()
    {
        timer = 0f;   // Ǯ���� ��Ȱ��ȭ �� Ÿ�̸� �ʱ�ȭ
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            PoolManager.Instance.ReturnToPool(gameObject); // Destroy -> ReturnToPool
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Wall"))
        {
            PoolManager.Instance.ReturnToPool(gameObject); // Destroy -> ReturnToPool
        }
    }
}
