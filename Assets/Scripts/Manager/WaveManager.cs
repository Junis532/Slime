using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class WaveManager : MonoBehaviour
{
    [Header("WaveData 리스트 (1 웨이브 = 1 WaveData)")]
    public List<WaveData> waveDataList;

    [Header("웨이브 스폰 설정")]
    public Transform playerTransform;
    public float spawnInterval = 5f;
    public float spawnRadius = 5f;
    public GameObject warningEffectPrefab;
    public float warningDuration = 1f;

    public TextMeshProUGUI waveText;
    public int currentWave = 1;

    private Coroutine spawnCoroutine;

    void Start()
    {
        ResetWave();
        StartSpawnLoop();
    }

    public void ResetWave()
    {
        currentWave = 1;
        UpdateWaveText();
    }

    public void UpdateWaveText()
    {
        if (waveText != null)
        {
            waveText.text = $"WAVE {currentWave}";
        }
    }

    public void StartNextWave()
    {
        if (currentWave >= waveDataList.Count)
        {
            Debug.LogWarning("더 이상 웨이브가 없습니다.");
            return;
        }

        currentWave++;
        UpdateWaveText();
        UpdateEnemyHP();

        StartCoroutine(SpawnWithWarning());

        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.ResetRerollPrice();
        }

        GameManager.Instance.ChangeStateToGame();

        RestartSpawnLoop();
    }

    IEnumerator SpawnWithWarning()
    {
        WaveData currentWaveData = waveDataList[currentWave - 1];

        if (currentWaveData == null || currentWaveData.enemyPrefabs.Count == 0)
        {
            Debug.LogWarning($"[WaveManager] 유효한 WaveData가 없습니다. currentWave = {currentWave}");
            yield break;
        }

        int spawnCount = Random.Range(currentWaveData.minSpawnCount, currentWaveData.maxSpawnCount + 1);
        List<(Vector2, GameObject)> spawnDataList = new List<(Vector2, GameObject)>();

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 spawnOffset = Random.insideUnitCircle.normalized * spawnRadius;
            Vector2 spawnPosition = (Vector2)playerTransform.position + spawnOffset;

            GameObject prefab = currentWaveData.enemyPrefabs[
                Random.Range(0, currentWaveData.enemyPrefabs.Count)];

            //  경고 이펙트 프리뷰 생성 대상 수집
            spawnDataList.Add((spawnPosition, prefab));
        }

        // 경고 이펙트 보여주기
        foreach (var (spawnPos, prefab) in spawnDataList)
        {
            // 일단 프리팹 인스턴스를 비활성화 상태로 만든다 (경고 위치 확인용)
            GameObject tempObj = Instantiate(prefab, spawnPos, Quaternion.identity);
            tempObj.SetActive(false);

            // 하위에 몬스터가 있는 경우 (그룹 프리팹)
            var allMonsters = tempObj.GetComponentsInChildren<Transform>();
            bool isGroup = allMonsters.Length > 1;

            foreach (var t in allMonsters)
            {
                if (t == tempObj.transform) continue;

                if (warningEffectPrefab != null)
                {
                    GameObject warning = Instantiate(warningEffectPrefab, t.position, Quaternion.identity);
                    SpriteRenderer sr = warning.GetComponent<SpriteRenderer>();
                    sr.color = new Color(1, 0, 0, 0);

                    sr.DOFade(1f, 0.3f)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetEase(Ease.InOutQuad);

                    Destroy(warning, warningDuration);
                }
            }

            // 자식이 없는 일반 몬스터일 경우
            if (!isGroup)
            {
                if (warningEffectPrefab != null)
                {
                    GameObject warning = Instantiate(warningEffectPrefab, spawnPos, Quaternion.identity);
                    SpriteRenderer sr = warning.GetComponent<SpriteRenderer>();
                    sr.color = new Color(1, 0, 0, 0);

                    sr.DOFade(1f, 0.3f)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetEase(Ease.InOutQuad);

                    Destroy(warning, warningDuration);
                }
            }

            // 경고 후 제거
            Destroy(tempObj);
        }

        yield return new WaitForSeconds(warningDuration);

        // 실제 스폰
        foreach (var (spawnPos, prefab) in spawnDataList)
        {
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }


    void UpdateEnemyHP()
    {
        float waveFactorEnemy = 0.07f + (currentWave / 30000f);
        float waveFactorLongRange = 0.068f + (currentWave / 30000f);

        int prevEnemyHP = GameManager.Instance.enemyStats.maxHP;
        int nextEnemyHP = Mathf.FloorToInt(prevEnemyHP + prevEnemyHP * waveFactorEnemy);
        GameManager.Instance.enemyStats.maxHP = nextEnemyHP;
        GameManager.Instance.enemyStats.currentHP = nextEnemyHP;

        int prevDashHP = GameManager.Instance.dashEnemyStats.maxHP;
        int nextDashHP = Mathf.FloorToInt(prevDashHP + prevDashHP * waveFactorEnemy);
        GameManager.Instance.dashEnemyStats.maxHP = nextDashHP;
        GameManager.Instance.dashEnemyStats.currentHP = nextDashHP;

        int prevLongRangeHP = GameManager.Instance.longRangeEnemyStats.maxHP;
        int nextLongRangeHP = Mathf.FloorToInt(prevLongRangeHP + prevLongRangeHP * waveFactorLongRange);
        GameManager.Instance.longRangeEnemyStats.maxHP = nextLongRangeHP;
        GameManager.Instance.longRangeEnemyStats.currentHP = nextLongRangeHP;

        int prevPotionHP = GameManager.Instance.potionEnemyStats.maxHP;
        int nextPotionHP = Mathf.FloorToInt(prevPotionHP + prevPotionHP * waveFactorLongRange);
        GameManager.Instance.potionEnemyStats.maxHP = nextPotionHP;
        GameManager.Instance.potionEnemyStats.currentHP = nextPotionHP;
    }

    IEnumerator SpawnerLoopRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(SpawnWithWarning());
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void StartSpawnLoop()
    {
        if (spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnerLoopRoutine());
        }
    }

    public void StopSpawnLoop()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    void RestartSpawnLoop()
    {
        StopSpawnLoop();
        StartSpawnLoop();
    }
}
