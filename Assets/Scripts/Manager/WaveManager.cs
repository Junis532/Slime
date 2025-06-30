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

        // ShopManager 싱글톤에서 리롤 가격 초기화 호출
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.ResetRerollPrice();
        }

        GameManager.Instance.ChangeStateToGame();
    }

    void ActivateWaveObject()
    {
        if (waveParent == null) return;

        string targetWaveName = $"Wave_{currentWave}";

        foreach (Transform child in waveParent.transform)
        {
            child.gameObject.SetActive(false);
        }

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
}
