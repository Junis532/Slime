using UnityEngine;
using System.Collections;

public class PotionEnemySpawner : MonoBehaviour
{
    [Header("���� ������ �� ����")]
    public GameObject[] enemyPrefabs;   // ���� ���� �� ������

    [Header("���� �ֱ�")]
    public float spawnInterval = 3f;

    [Header("���� ���� (���� �ݰ�)")]
    public float spawnRadius = 3f;

    [Header("�� �׷�� ���� ����")]
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
            // ���� �� ���� ����
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            // ���� ���� �� ���� ��ġ
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
