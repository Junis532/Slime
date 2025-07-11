using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("WaveData ����Ʈ (1 ���̺� = 1 WaveData)")]
    public List<WaveData> waveDataList;

    [Header("���̺� ������ ���� ����")]
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
            Debug.LogWarning("�� �̻� ���̺갡 �����ϴ�.");
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
        // ���� ���̺꿡 �ش��ϴ� WaveData ��������
        WaveData currentWaveData = waveDataList[currentWave - 1]; // 0-based �ε���

        if (currentWaveData == null)
        {
            Debug.LogWarning($"[WaveManager] WaveData�� �������� �ʽ��ϴ�. currentWave = {currentWave}");
            return;
        }

        // spawnerCount ����ŭ ����
        for (int i = 0; i < spawnerCount; i++)
        {
            if (currentWaveData.spawnerGroupPrefabs.Count == 0) break;

            Vector2 randomOffset = Random.insideUnitCircle.normalized * spawnerRadius;
            Vector2 spawnPos = (Vector2)playerTransform.position + randomOffset;

            // spawnerGroupPrefabs �� �������� �ϳ� ����
            GameObject selectedPrefab = currentWaveData.spawnerGroupPrefabs[Random.Range(0, currentWaveData.spawnerGroupPrefabs.Count)];
            GameObject spawnerObj = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);

            // �ڽ� �����Ͽ� EnemySpawner ���� ã��
            EnemySpawner[] spawners = spawnerObj.GetComponentsInChildren<EnemySpawner>();
            if (spawners.Length > 0)
            {
                foreach (var spawner in spawners)
                {
                    spawner.StartSpawning();
                    Debug.Log($"[WaveManager] EnemySpawner ���۵�: {spawner.name}");
                }
            }
            else
            {
                Debug.LogWarning($"[WaveManager] EnemySpawner�� ã�� �� ����. ������ �̸�: {spawnerObj.name}");
            }
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
