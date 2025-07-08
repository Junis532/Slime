using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MucusArea : MonoBehaviour
{
    private float slowRatio = 0.5f; // �ӵ��� 50%�� ����
    private bool isSlowing = false;

    // ���� �� ������ ���� ����
    private List<EnemyBase> enemiesInRange = new List<EnemyBase>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("DashEnemy") ||
            other.CompareTag("LongRangeEnemy") || other.CompareTag("PotionEnemy"))
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>(); // ���� �θ� Ŭ���� �Ǵ� �������̽�
            if (enemy != null && !enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
                enemy.speed = enemy.originalSpeed * slowRatio;

                if (!isSlowing)
                {
                    StartCoroutine(SlowOverTime());
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("DashEnemy") ||
            other.CompareTag("LongRangeEnemy") || other.CompareTag("PotionEnemy"))
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            if (enemy != null && enemiesInRange.Contains(enemy))
            {
                enemy.speed = enemy.originalSpeed; // ���� �ӵ��� ����
                enemiesInRange.Remove(enemy);

                if (enemiesInRange.Count == 0)
                {
                    isSlowing = false;
                    StopCoroutine(SlowOverTime());
                }
            }
        }
    }

    private IEnumerator SlowOverTime()
    {
        isSlowing = true;

        while (isSlowing)
        {
            enemiesInRange.RemoveAll(e => e == null);

            foreach (var enemy in enemiesInRange)
            {
                enemy.speed = enemy.originalSpeed * slowRatio; // �ӵ� ����
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}