using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonDamage : MonoBehaviour
{
    private int damage;
    private List<EnemyHP> enemiesInRange = new List<EnemyHP>();
    private bool isDamaging = false;

    public void Init()
    {
        damage = Mathf.FloorToInt(GameManager.Instance.playerStats.attack * 1f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("DashEnemy") ||
            other.CompareTag("LongRangeEnemy") || other.CompareTag("PotionEnemy"))
        {
            EnemyHP hp = other.GetComponent<EnemyHP>();
            if (hp != null && !enemiesInRange.Contains(hp))
            {
                enemiesInRange.Add(hp);

                // ������ �ִ� �ڷ�ƾ�� ���� ���� �ƴϸ� ����
                if (!isDamaging)
                {
                    StartCoroutine(DamageOverTime());
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("DashEnemy") ||
            other.CompareTag("LongRangeEnemy") || other.CompareTag("PotionEnemy"))
        {
            EnemyHP hp = other.GetComponent<EnemyHP>();
            if (hp != null && enemiesInRange.Contains(hp))
            {
                enemiesInRange.Remove(hp);

                // ���� �� �� ������ �ڷ�ƾ ����
                if (enemiesInRange.Count == 0)
                {
                    isDamaging = false;
                    StopCoroutine(DamageOverTime());
                }
            }
        }
    }

    private IEnumerator DamageOverTime()
    {
        isDamaging = true;

        while (isDamaging)
        {
            for (int i = enemiesInRange.Count - 1; i >= 0; i--)
            {
                EnemyHP enemy = enemiesInRange[i];
                if (enemy == null)
                {
                    enemiesInRange.RemoveAt(i);
                    continue;
                }

                enemy.SkillTakeDamage(damage);
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
