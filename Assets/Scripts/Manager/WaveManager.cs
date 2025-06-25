using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Wave 오브젝트")]
    public GameObject Wave1;
    public GameObject Wave3;
    public GameObject Wave4;

    public TextMeshProUGUI waveText;
    public int currentWave = 1;

    public void UpdateWaveText()
    {
        if (waveText != null)
        {
            waveText.text = $"WAVE {currentWave}";
        }
    }

    [System.Obsolete]
    public void StartNextWave()
    {
        currentWave++;

        UpdateWaveText();

        // --------- 각 Enemy 타입 HP 증가 적용 ---------
        UpdateEnemyHP();

        // --------- 웨이브 오브젝트 활성화 ---------
        if (Wave1 != null) Wave1.SetActive(false);
        if (Wave3 != null) Wave3.SetActive(false);
        if (Wave4 != null) Wave4.SetActive(false);

        if (currentWave >= 1 && currentWave <= 2)
        {
            if (Wave1 != null) Wave1.SetActive(true);
        }
        else if (currentWave == 3)
        {
            if (Wave3 != null) Wave3.SetActive(true);
        }
        else if (currentWave >= 4 && currentWave <= 13)
        {
            if (Wave4 != null) Wave4.SetActive(true);
        }

        GameManager.Instance.ChangeStateToGame();
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
