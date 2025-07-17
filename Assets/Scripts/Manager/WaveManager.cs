using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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

    [Header("★ 반드시 인스펙터/코드로 연결")]
    public JoystickDirectionIndicator3 playerSkillController;   // << 추가 >>

    private Coroutine spawnCoroutine;

    void Start()
    {
        if (playerSkillController == null)
            playerSkillController = FindFirstObjectByType<JoystickDirectionIndicator3>(); 
        Debug.Log("[WaveManager] 스킬 순서: " + string.Join(", ", SkillSelect.FinalSkillOrder));
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
            waveText.text = $"WAVE {currentWave}";
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
            ShopManager.Instance.ResetRerollPrice();
        GameManager.Instance.ChangeStateToGame();
        RestartSpawnLoop();
    }

    bool IsEnemyTag(string tag)
    {
        return tag == "Enemy" || tag == "DashEnemy" || tag == "LongRangeEnemy" || tag == "PotionEnemy";
    }

    IEnumerator SpawnWithWarning()
    {
        WaveData currentWaveData = waveDataList[currentWave - 1];
        if (currentWaveData == null || currentWaveData.skillMonsterLists.Count == 0)
        {
            Debug.LogWarning($"[WaveManager] 유효한 WaveData가 없습니다. currentWave = {currentWave}");
            yield break;
        }

        int currentSkillNumber = playerSkillController != null ? playerSkillController.CurrentUsingSkillIndex : 0;
        if (currentSkillNumber <= 0)
        {
            Debug.LogWarning("[WaveManager] 현재 사용 중인 스킬이 없음: 몬스터 스폰 스킵");
            yield break;
        }

        int skillListIndex = currentSkillNumber - 1;
        if (skillListIndex < 0 || skillListIndex >= currentWaveData.skillMonsterLists.Count)
        {
            Debug.LogWarning($"[WaveManager] 스킬({currentSkillNumber})에 매핑된 몬스터 리스트 없음");
            yield break;
        }

        var monsterList = currentWaveData.skillMonsterLists[skillListIndex].monsters;
        if (monsterList == null || monsterList.Count == 0)
        {
            Debug.LogWarning($"[WaveManager] 스킬({currentSkillNumber})의 랜덤 몬스터 리스트가 비어있음");
            yield break;
        }

        int spawnCount = Random.Range(currentWaveData.minSpawnCount, currentWaveData.maxSpawnCount + 1);
        List<GameObject> spawnMonsters = new List<GameObject>();
        List<Vector2> spawnPositions = new List<Vector2>();
        for (int i = 0; i < spawnCount; i++)
        {
            // 각 몬스터는 랜덤
            GameObject selected = monsterList[Random.Range(0, monsterList.Count)];
            spawnMonsters.Add(selected);

            Vector2 spawnOffset = Random.insideUnitCircle.normalized * spawnRadius;
            Vector2 spawnPosition = (Vector2)playerTransform.position + spawnOffset;
            spawnPositions.Add(spawnPosition);
        }

        // 경고 이펙트
        // 경고 이펙트(적 그룹이면 여러 개, 싱글이면 1개)
        for (int i = 0; i < spawnPositions.Count; i++)
        {
            GameObject prefab = spawnMonsters[i];
            Vector2 spawnPos = spawnPositions[i];

            GameObject tempObj = Instantiate(prefab, spawnPos, Quaternion.identity);
            tempObj.SetActive(false);

            bool hasRealEnemy = false;
            var allMonsters = tempObj.GetComponentsInChildren<Transform>();
            foreach (var t in allMonsters)
            {
                if (t == tempObj.transform) continue;
                if (IsEnemyTag(t.gameObject.tag) && warningEffectPrefab != null)
                {
                    // 몬스터 본체(자식들) 위치에 이펙트 생성
                    GameObject warning = Instantiate(warningEffectPrefab, t.position, Quaternion.identity);
                    SpriteRenderer sr = warning.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        sr.color = new Color(1, 0, 0, 0);
                        sr.DOFade(1f, 0.3f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
                    }
                    Destroy(warning, warningDuration);
                    hasRealEnemy = true; // 적 자식이 하나라도 있으면
                }
            }
            // 자식 중 Enemy가 하나도 없으면, 자기 자신 위치에
            if (!hasRealEnemy && IsEnemyTag(tempObj.tag) && warningEffectPrefab != null)
            {
                GameObject warning = Instantiate(warningEffectPrefab, spawnPos, Quaternion.identity);
                SpriteRenderer sr = warning.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = new Color(1, 0, 0, 0);
                    sr.DOFade(1f, 0.3f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
                }
                Destroy(warning, warningDuration);
            }
            Destroy(tempObj);
        }
        yield return new WaitForSeconds(warningDuration);


        // 몬스터 스폰
        for (int i = 0; i < spawnPositions.Count; i++)
            Instantiate(spawnMonsters[i], spawnPositions[i], Quaternion.identity);
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
        float initialDelay = 1f;
        yield return new WaitForSeconds(initialDelay);
        while (true)
        {
            yield return StartCoroutine(SpawnWithWarning());
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void StartSpawnLoop()
    {
        if (spawnCoroutine == null)
            spawnCoroutine = StartCoroutine(SpawnerLoopRoutine());
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
