using UnityEngine;

public class MeteorDamage : MonoBehaviour
{
    private int damage;

    public void Init()
    {
        damage = Mathf.FloorToInt(GameManager.Instance.playerStats.attack * 2f);
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
            }
        }
    }
}
