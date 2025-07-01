using System.Collections;
using UnityEngine;

public class PoisonSpawner : MonoBehaviour
{
    public GameObject poisonPrefab;   // 포션 적 프리팹
    public GameObject player;               // 플레이어 오브젝트 할당

    public float spawnInterval = 15f;
    public float poisonLifetime = 10f;
    public Vector3 spawnOffset = Vector3.zero;  // 플레이어 위치 기준 오프셋 (발 밑 조정용)

    private Coroutine spawnCoroutine;

    void Start()
    {
        if (poisonPrefab != null && player != null)
        {
            spawnCoroutine = StartCoroutine(SpawnPotionEnemyRoutine());
        }
        else
        {
            Debug.LogWarning("PotionEnemy 프리팹 또는 Player 오브젝트가 할당되지 않았습니다.");
        }
    }

    private IEnumerator SpawnPotionEnemyRoutine()
    {
        while (true)
        {
            // 플레이어 현재 위치 + 오프셋 (발 밑 위치에 맞게 조정)
            Vector3 spawnPos = player.transform.position + spawnOffset;

            GameObject potion = Instantiate(poisonPrefab, spawnPos, Quaternion.identity);

            // 데미지 초기화
            PoisonDamage poisonDamage = potion.GetComponent<PoisonDamage>();
            if (poisonDamage != null)
            {
                poisonDamage.Init();
            }

            Destroy(potion, poisonLifetime);

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void OnDisable()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
    }
}
