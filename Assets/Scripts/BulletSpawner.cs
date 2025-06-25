using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [Header("🔫 총알 프리팹")]
    public GameObject bulletPrefab;

    [Header("🕒 생성 간격")]
    public float spawnInterval = 2f;

    [Header("📏 플레이어 앞쪽 거리 오프셋")]
    public float spawnOffset = 0.5f;

    private float timer;

    void Update()
    {
        // 게임 상태가 Game일 때만 총알 소환
        if (!GameManager.Instance.IsGame())
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnBullet();
            timer = 0f;
        }
    }

    void SpawnBullet()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && bulletPrefab != null)
        {
            Vector3 spawnPos = player.transform.position;

            SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                float direction = sr.flipX ? -1f : 1f;
                spawnPos += new Vector3(spawnOffset * direction, 0f, 0f);
            }

            Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Player 또는 Bullet Prefab이 없습니다.");
        }
    }
}
