using UnityEngine;

public class ZacPiece : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.playerStats.currentHP += 1;
            Destroy(gameObject);
        }
        else if (other.CompareTag("Enemy") || other.CompareTag("DashEnemy") ||
                 other.CompareTag("LongRangeEnemy") || other.CompareTag("PotionEnemy"))
        {
            Destroy(gameObject);
        }
    }
}
