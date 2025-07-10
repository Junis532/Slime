using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("WaveData 리스트 (1 웨이브 = 1 WaveData)")]
    public List<WaveData> waveDataList;

    [Header("웨이브 스포너 생성 관련")]
    public Transform playerTransform;
    public float spawnInterval = 5f;
    public float spawnerRadius = 6f;
    public int spawnerCount = 3;

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

        ActivateWaveSpawnerGroups();

        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.ResetRerollPrice();
        }

        GameManager.Instance.ChangeStateToGame();

        RestartSpawnLoop();
    }

    void ActivateWaveSpawnerGroups()
    {
        // 현재 웨이브에 해당하는 WaveData 가져오기
        WaveData currentWaveData = waveDataList[currentWave - 1]; // 0-based 인덱스

        if (currentWaveData == null)
        {
            Debug.LogWarning($"[WaveManager] WaveData가 존재하지 않습니다. currentWave = {currentWave}");
            return;
        }

        // 여기서는 현재 WaveData가 가진 spawnerGroupPrefabs 리스트를
        // spawnerCount만큼 플레이어 주변에 생성하는 작업으로 변경

        //// 기존에 있는 스포너들은 모두 삭제하거나 관리 필요 (간단하게 삭제)
        //foreach (var existingSpawner in GameObject.FindGameObjectsWithTag("EnemySpawner"))
        //{
        //    Destroy(existingSpawner);
        //}

        // spawnerCount 수만큼 생성
        for (int i = 0; i < spawnerCount; i++)
        {
            if (currentWaveData.spawnerGroupPrefabs.Count == 0) break;

            Vector2 randomOffset = Random.insideUnitCircle.normalized * spawnerRadius;
            Vector2 spawnPos = (Vector2)playerTransform.position + randomOffset;

            // spawnerGroupPrefabs 중 랜덤으로 하나 선택
            GameObject selectedPrefab = currentWaveData.spawnerGroupPrefabs[Random.Range(0, currentWaveData.spawnerGroupPrefabs.Count)];
            GameObject spawnerObj = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);

            // EnemySpawner 컴포넌트 찾아서 스폰 시작
            EnemySpawner spawner = spawnerObj.GetComponent<EnemySpawner>();
            if (spawner != null)
                spawner.StartSpawning();
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
            ActivateWaveSpawnerGroups();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void StartSpawnLoop()
    {
        if (spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnerLoopRoutine());
        }
    }

    void StopSpawnLoop()
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
