using UnityEngine;

public class LongRangeEnemyBullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 데미지 주기
            GameManager.Instance.playerStats.currentHP -= GameManager.Instance.enemyStats.attack;

            // 체력이 0 이하이면 죽음 처리
            if (GameManager.Instance.playerStats.currentHP <= 0)
            {
                GameManager.Instance.playerStats.currentHP = 0;
                //GameManager.Instance.PlayerDie?.Invoke(); // 함수가 있다면 호출
            }

            // 자기 자신(총알 등) 삭제
            Destroy(gameObject);
        }
    }
}
