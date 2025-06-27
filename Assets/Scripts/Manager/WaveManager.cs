using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Wave들의 부모 오브젝트")]
    public GameObject waveParent;

    public TextMeshProUGUI waveText;
    public int currentWave = 1;

    public void UpdateWaveText()
    {
        if (waveText != null)
        {
            waveText.text = $"WAVE {currentWave}";
        }
    }

    public void StartNextWave()
    {
        currentWave++;

        UpdateWaveText();

        UpdateEnemyHP();

        ActivateWaveObject();

        GameManager.Instance.ChangeStateToGame();
    }

    void ActivateWaveObject()
    {
        if (waveParent == null) return;

        string targetWaveName = $"Wave_{currentWave}";

        // 먼저 모든 자식 비활성화
        foreach (Transform child in waveParent.transform)
        {
            child.gameObject.SetActive(false);
        }

        // 해당 웨이브 이름 가진 자식만 활성화
        Transform targetWave = waveParent.transform.Find(targetWaveName);
        if (targetWave != null)
        {
            targetWave.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"[WaveManager] {targetWaveName} 오브젝트를 찾을 수 없습니다.");
        }
    }

    void UpdateEnemyHP()
    {
        float waveFactorEnemy = 0.07f + (currentWave / 30000f);
        float waveFactorLongRange = 0.068f + (currentWave / 30000f);

        // Enemy
        int prevEnemyHP = GameManager.Instance.enemyStats.maxHP;
        int nextEnemyHP = Mathf.FloorToInt(prevEnemyHP + prevEnemyHP * waveFactorEnemy);
        GameManager.Instance.enemyStats.maxHP = nextEnemyHP;
        GameManager.Instance.enemyStats.currentHP = nextEnemyHP;

        // DashEnemy
        int prevDashHP = GameManager.Instance.dashEnemyStats.maxHP;
        int nextDashHP = Mathf.FloorToInt(prevDashHP + prevDashHP * waveFactorEnemy);
        GameManager.Instance.dashEnemyStats.maxHP = nextDashHP;
        GameManager.Instance.dashEnemyStats.currentHP = nextDashHP;

        // LongRangeEnemy
        int prevLongRangeHP = GameManager.Instance.longRangeEnemyStats.maxHP;
        int nextLongRangeHP = Mathf.FloorToInt(prevLongRangeHP + prevLongRangeHP * waveFactorLongRange);
        GameManager.Instance.longRangeEnemyStats.maxHP = nextLongRangeHP;
        GameManager.Instance.longRangeEnemyStats.currentHP = nextLongRangeHP;

        // PotionEnemy
        int prevPotionHP = GameManager.Instance.potionEnemyStats.maxHP;
        int nextPotionHP = Mathf.FloorToInt(prevPotionHP + prevPotionHP * waveFactorLongRange);
        GameManager.Instance.potionEnemyStats.maxHP = nextPotionHP;
        GameManager.Instance.potionEnemyStats.currentHP = nextPotionHP;
    }
}
