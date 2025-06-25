using System.Collections;
using UnityEngine;

public class PotionEnemyDamage : MonoBehaviour
{
    private bool isTakingDamage = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!isTakingDamage)
            {
                StartCoroutine(DamageOverTime());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isTakingDamage = false;
        }
    }

    private IEnumerator DamageOverTime()
    {
        isTakingDamage = true;

        while (isTakingDamage && GameManager.Instance.playerStats.currentHP > 0)
        {
            GameManager.Instance.playerStats.currentHP -= 1;

            if (GameManager.Instance.playerStats.currentHP <= 0)
            {
                GameManager.Instance.playerStats.currentHP = 0;
                // 플레이어 죽음 처리 (있다면)
                // GameManager.Instance.PlayerDie?.Invoke();
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
