using UnityEngine;

public class BossFireballProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 2f;
    private int damage;

    private Vector2 direction;

    public void Init(Vector2 dir)
    {
        direction = dir.normalized;

        // Fireball 생성 시 현재 공격력을 기반으로 데미지 계산
        damage = Mathf.FloorToInt(GameManager.Instance.playerStats.attack * 2.5f);

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            int damage = GameManager.Instance.enemyStats.attack;
            GameManager.Instance.playerStats.currentHP -= damage;
            GameManager.Instance.playerDamaged.PlayDamageEffect(); // 플레이어 데미지 이펙트 재생

            if (GameManager.Instance.playerStats.currentHP <= 0)
            {
                GameManager.Instance.playerStats.currentHP = 0;
                // 죽음 처리 함수 호출 가능
            }
        }
    }
}
