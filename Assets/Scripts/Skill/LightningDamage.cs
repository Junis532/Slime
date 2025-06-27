using UnityEngine;

public class LightningDamage : MonoBehaviour
{
    private int damage;

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
        }
    }
}
