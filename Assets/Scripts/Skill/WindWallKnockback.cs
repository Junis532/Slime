using UnityEngine;
using System.Collections.Generic;

public class WindWallKnockback : MonoBehaviour
{

    [Header("������ �� ���ط� (�÷��̾� ���ݷ� ���)")]
    public float damageMultiplier = 0.1f;

    private int damage;

    [Header("�ٶ��� ���ӽð�")]
    public float lifetime = 3f;

    [Header("���� ������ ����(��)")]
    public float damageInterval = 0.5f;  // 0.5�ʸ��� ������

    // ���� ������ ��Ÿ�� ������
    private Dictionary<Collider2D, float> damageTimers = new Dictionary<Collider2D, float>();

    void Start()
    {
        damage = Mathf.FloorToInt(GameManager.Instance.playerStats.attack * damageMultiplier);
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsNotPlayer(collision.collider))
        {
            
            EnemyHP hp = collision.collider.GetComponent<EnemyHP>();
            if (hp != null)
            {
                hp.SkillTakeDamage(damage);
            }

            damageTimers[collision.collider] = 0f;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (IsNotPlayer(collision.collider))
        {
            if (!damageTimers.ContainsKey(collision.collider))
                damageTimers[collision.collider] = 0f;

            damageTimers[collision.collider] += Time.deltaTime;

            if (damageTimers[collision.collider] >= damageInterval)
            {
                EnemyHP hp = collision.collider.GetComponent<EnemyHP>();
                if (hp != null)
                {
                    hp.SkillTakeDamage(damage);
                }

                damageTimers[collision.collider] = 0f;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (damageTimers.ContainsKey(collision.collider))
        {
            damageTimers.Remove(collision.collider);
        }
    }

    private bool IsNotPlayer(Collider2D other)
    {
        return !other.CompareTag("Player");
    }
}
