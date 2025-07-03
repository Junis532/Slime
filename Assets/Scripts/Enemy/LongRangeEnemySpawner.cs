using UnityEngine;
using System.Collections;
using DG.Tweening;

public class LongRangeEnemySpawner : MonoBehaviour
{
    [Header("스폰 가능한 적 종류")]
    public GameObject[] enemyPrefabs;

    [Header("경고 이펙트 프리팹")]
    public GameObject warningEffectPrefab;  // 경고 이미지 프리팹

    [Header("스폰 주기")]
    public float spawnInterval = 3f;

    [Header("스폰 범위 (원형 반경)")]
    public float spawnRadius = 3f;

    [Header("한 그룹당 스폰 개수")]
    public int minSpawnCount = 3;
    public int maxSpawnCount = 6;

    [Header("경고 지속 시간")]
    public float warningDuration = 1.5f;

    private Coroutine spawnCoroutine;

    IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            yield return SpawnEnemyGroupWithWarning();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator SpawnEnemyGroupWithWarning()
    {
        int spawnCount = Random.Range(minSpawnCount, maxSpawnCount + 1);
        Vector2[] spawnPositions = new Vector2[spawnCount];

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
            spawnPositions[i] = randomPos;

            if (warningEffectPrefab != null)
            {
                GameObject warning = Instantiate(warningEffectPrefab, randomPos, Quaternion.identity);
                SpriteRenderer sr = warning.GetComponent<SpriteRenderer>();

                if (sr != null)
                {
                    sr.color = new Color(1f, 0f, 0f, 0f);
                    sr.DOFade(1f, 0.3f)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetEase(Ease.InOutQuad);
                }

                Destroy(warning, warningDuration);
            }
        }

        yield return new WaitForSeconds(warningDuration);

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Instantiate(enemyPrefab, spawnPositions[i], Quaternion.identity);
        }
    }

    public void StartSpawning()
    {
        if (spawnCoroutine == null)
            spawnCoroutine = StartCoroutine(SpawnEnemyRoutine());
    }

    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }
}
