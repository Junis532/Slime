using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class EnemySpawner : MonoBehaviour
{
    [Header("스폰 가능한 적 종류")]
    public List<GameObject> enemyPrefabs = new List<GameObject>();

    [Header("경고 이펙트")]
    public GameObject warningEffectPrefab;

    [Header("스폰 범위 (원형 반경)")]
    public float spawnRadius = 3f;

    [Header("한 그룹당 스폰 개수")]
    public int minSpawnCount = 3;
    public int maxSpawnCount = 6;

    [Header("스폰 딜레이")]
    public float warningDuration = 1.5f; // 경고 이미지가 깜빡이는 시간

    private Coroutine spawnCoroutine;

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
            sr.color = new Color(1, 0, 0, 0); // 투명하게 시작

            sr.DOFade(1f, 0.3f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutQuad);

            Destroy(warning, warningDuration);
        }

        yield return new WaitForSeconds(warningDuration);

        foreach (Vector2 pos in spawnPositions)
        {
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            Instantiate(enemyPrefab, pos, Quaternion.identity);
        }

        spawnCoroutine = null;

        // ✅ 스폰 후 스포너 오브젝트 삭제
        Destroy(gameObject);
    }

    public void StartSpawning()
    {
        if (spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnEnemyGroupWithWarning());
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
