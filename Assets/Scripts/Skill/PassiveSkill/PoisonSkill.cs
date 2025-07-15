using System.Collections;
using UnityEngine;

public class PoisonSkill : MonoBehaviour
{
    public GameObject poisonPrefab;           // 생성할 독 프리팹
    public float spawnInterval = 15f;         // 생성 간격
    public float poisonLifetime = 10f;        // 독 지속 시간
    public Vector3 spawnOffset = Vector3.zero; // 발 밑 위치 조정

    private Coroutine spawnCoroutine;

    void Start()
    {
        if (poisonPrefab != null)
        {
            spawnCoroutine = StartCoroutine(SpawnPoisonRoutine());
        }
        else
        {
            Debug.LogWarning("poisonPrefab이 설정되지 않았습니다.");
        }
    }

    private IEnumerator SpawnPoisonRoutine()
    {
        while (true)
        {
            Vector3 spawnPos = transform.position + spawnOffset;

            GameObject poison = Instantiate(poisonPrefab, spawnPos, Quaternion.identity);

            // 초기화
            PoisonDamage poisonDamage = poison.GetComponent<PoisonDamage>();
            if (poisonDamage != null)
            {
                poisonDamage.Init();
            }

            Destroy(poison, poisonLifetime);

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void OnDisable()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }
}
