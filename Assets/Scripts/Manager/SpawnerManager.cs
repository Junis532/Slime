using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class SpawnerManager : MonoBehaviour
{
    [Header("�÷��̾�")]
    public Transform playerTransform;

    [Header("���� �ֱ�")]
    public float spawnInterval = 5f; // ������ ���� �ֱ�

    [Header("������ ������ ������")]
    public List<GameObject> enemySpawnerPrefab;

    [Header("������ ���� �ݰ�")]
    public float spawnerRadius = 6f;

    [Header("������ ������ ��")]
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
            Debug.LogWarning("�÷��̾� �Ǵ� ������ �������� �Ҵ���� �ʾҽ��ϴ�.");
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
