using UnityEngine;

public class LongRangeEnemyBullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // ������ �ֱ�
            GameManager.Instance.playerStats.currentHP -= GameManager.Instance.enemyStats.attack;

            // ü���� 0 �����̸� ���� ó��
            if (GameManager.Instance.playerStats.currentHP <= 0)
            {
                GameManager.Instance.playerStats.currentHP = 0;
                //GameManager.Instance.PlayerDie?.Invoke(); // �Լ��� �ִٸ� ȣ��
            }

            // �ڱ� �ڽ�(�Ѿ� ��) ����
            Destroy(gameObject);
        }
    }
}
