using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class SpawnerManager : MonoBehaviour
{
    [Header("플레이어")]
    public Transform playerTransform;

    [Header("스폰 주기")]
    public float spawnInterval = 5f; // 스포너 생성 주기

    [Header("생성할 스포너 프리팹")]
    public List<GameObject> enemySpawnerPrefab;

    [Header("스포너 생성 반경")]
    public float spawnerRadius = 6f;

    [Header("생성할 스포너 수")]
    public int spawnerCount = 3;

    private Coroutine spawnCoroutine;

    void Start()
    {
        spawnCoroutine = StartCoroutine(SpawnerLoopRoutine());
    }

    public void SpawnSpawnersAroundPlayer()
    {
        if (playerTransform == null || enemySpawnerPrefab == null)
        {
            Debug.LogWarning("플레이어 또는 스포너 프리팹이 할당되지 않았습니다.");
            return;
        }

        for (int i = 0; i < spawnerCount; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle.normalized * spawnerRadius;
            Vector2 spawnPos = (Vector2)playerTransform.position + randomOffset;

            GameObject selectedPrefab = enemySpawnerPrefab[Random.Range(0, enemySpawnerPrefab.Count)];
            GameObject spawnerObj = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
            EnemySpawner spawner = spawnerObj.GetComponent<EnemySpawner>();

            if (spawner != null)
                spawner.StartSpawning();
        }
    }

    IEnumerator SpawnerLoopRoutine()
    {
        while (true)
        {
            SpawnSpawnersAroundPlayer();
            yield return new WaitForSeconds(spawnInterval);
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
