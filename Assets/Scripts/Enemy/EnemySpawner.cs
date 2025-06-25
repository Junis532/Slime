using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;

public class EnemySpawner : MonoBehaviour
{
    [Header("스폰 가능한 적 종류")]
    public List<GameObject> enemyPrefabs = new List<GameObject>();   // 여러 종류 적 프리팹

    [Header("스폰 주기")]
    public float spawnInterval = 3f;

    [Header("스폰 범위 (원형 반경)")]
    public float spawnRadius = 3f;

    [Header("한 그룹당 스폰 개수")]
    public int minSpawnCount = 3;
    public int maxSpawnCount = 6;

    private Coroutine spawnCoroutine;

    IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            SpawnEnemyGroup();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemyGroup()
    {
        int spawnCount = Random.Range(minSpawnCount, maxSpawnCount + 1);

        for (int i = 0; i < spawnCount; i++)
        {
            // 랜덤 적 종류 선택
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

            // 원형 범위 내 랜덤 위치
            Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;

            Instantiate(enemyPrefab, randomPos, Quaternion.identity);
        }
    }

    public void StartSpawning()
    {
        if (spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnEnemyRoutine());
        }
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
