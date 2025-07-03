using UnityEngine;
using System.Collections;
using DG.Tweening;

public class LongRangeEnemySpawner : MonoBehaviour
{
    [Header("���� ������ �� ����")]
    public GameObject[] enemyPrefabs;

    [Header("��� ����Ʈ ������")]
    public GameObject warningEffectPrefab;  // ��� �̹��� ������

    [Header("���� �ֱ�")]
    public float spawnInterval = 3f;

    [Header("���� ���� (���� �ݰ�)")]
    public float spawnRadius = 3f;

    [Header("�� �׷�� ���� ����")]
    public int minSpawnCount = 3;
    public int maxSpawnCount = 6;

    [Header("��� ���� �ð�")]
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
