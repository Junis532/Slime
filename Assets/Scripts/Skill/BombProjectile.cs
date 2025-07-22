using UnityEngine;

public class BombProjectile : MonoBehaviour
{
    public float speed = 4f;
    public float lifeTime = 2.5f;

    public float explosionRadius = 2f;               // ���� �ݰ�
    public GameObject explosionEffect;               // ���� ����Ʈ ������ (�ɼ�)

    private int damage;
    private Vector2 direction;

    public void Init(Vector2 dir)
    {
        direction = dir.normalized;

        // �÷��̾� ���ݷ� ��� ������ ���� (��: 3��)
        damage = Mathf.FloorToInt(GameManager.Instance.playerStats.attack * 0.5f);

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsEnemyTag(other.tag))
        {
            Explode();
        }
    }

    void Explode()
    {
        // ���� ����Ʈ ����
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // �ݰ� ���� �� Ž��
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hits)
        {
            if (IsEnemyTag(hit.tag))
            {
                EnemyHP hp = hit.GetComponent<EnemyHP>();
                if (hp != null)
                {
                    hp.SkillTakeDamage(damage);
                    Debug.Log($"Bomb explosion hit {hit.name}, dealt {damage} damage.");
                }
            }
        }

        Destroy(gameObject); // ��ü ����
    }

    bool IsEnemyTag(string tag)
    {
        return tag == "Enemy" || tag == "DashEnemy" || tag == "LongRangeEnemy" || tag == "PotionEnemy";
    }

    void OnDrawGizmosSelected()
    {
        // Scene �信�� ���� �ݰ� �ð�ȭ
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
