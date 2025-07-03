using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class EnemySpawner : MonoBehaviour
{
    [Header("���� ������ �� ����")]
    public List<GameObject> enemyPrefabs = new List<GameObject>();

    [Header("��� ����Ʈ")]
    public GameObject warningEffectPrefab;

    [Header("���� �ֱ�")]
    public float spawnInterval = 3f;

    [Header("���� ���� (���� �ݰ�)")]
    public float spawnRadius = 3f;

    [Header("�� �׷�� ���� ����")]
    public int minSpawnCount = 3;
    public int maxSpawnCount = 6;

    [Header("���� ������")]
    public float warningDuration = 1.5f; // ��� �̹����� �����̴� �ð�

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

        List<Vector2> spawnPositions = new List<Vector2>();

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
            spawnPositions.Add(randomPos);

            GameObject warning = Instantiate(warningEffectPrefab, randomPos, Quaternion.identity);
            SpriteRenderer sr = warning.GetComponent<SpriteRenderer>();
            sr.color = new Color(1, 0, 0, 0); // �����ϰ� ����

            // DOTween ���� �ݺ� �ִϸ��̼�
            sr.DOFade(1f, 0.3f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutQuad);

            // ���� �ð� �� ����
            Destroy(warning, warningDuration);
        }

        yield return new WaitForSeconds(warningDuration);

        // ��� �� ���� �� ����
        foreach (Vector2 pos in spawnPositions)
        {
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            Instantiate(enemyPrefab, pos, Quaternion.identity);
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
