using UnityEngine;

public class LightningDamage : MonoBehaviour
{
    private int damage;

    [Header("�¾��� �� ǥ���� ����Ʈ ������")]
    public GameObject hitEffectPrefab;

    // ������ �ʱ�ȭ (���ӸŴ������� ���ݷ� �����ͼ� ���)
    public void Init()
    {
        damage = Mathf.FloorToInt(GameManager.Instance.playerStats.attack * 2.5f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("DashEnemy") ||
            other.CompareTag("LongRangeEnemy") || other.CompareTag("PotionEnemy"))
        {
            EnemyHP hp = other.GetComponent<EnemyHP>();
            if (hp != null)
            {
                hp.SkillTakeDamage(damage);
                Debug.Log($"Lightning hit {other.name}, dealt {damage} damage.");
            }

            if (hitEffectPrefab != null)
            {
                GameObject effect = Instantiate(hitEffectPrefab, other.transform.position, Quaternion.identity);
                Destroy(effect, 0.3f);
            }
        }
    }
}
