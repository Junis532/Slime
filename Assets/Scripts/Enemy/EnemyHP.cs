using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public GameObject hpBarPrefab; // EnemyHPBar ������
    private EnemyHPBar hpBar;
    private float currentHP;
    private float maxHP;

    [System.Obsolete]
    void Start()
    {
        maxHP = GameManager.Instance.enemyStats.maxHP;
        currentHP = maxHP;

        // HP�� ���� �� �ʱ�ȭ
        Canvas worldCanvas = FindObjectOfType<Canvas>(); // ����ĵ���� ã�Ƽ�
        GameObject hpBarObj = Instantiate(hpBarPrefab, worldCanvas.transform);
        hpBar = hpBarObj.GetComponent<EnemyHPBar>();
        hpBar.Init(transform, maxHP);
        hpBarObj.SetActive(false); // ó���� �� ���̰�
    }

    public void TakeDamage()
    {
        currentHP -= GameManager.Instance.playerStats.attack;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        if (hpBar != null)
        {
            hpBar.SetHP(currentHP);
            hpBar.gameObject.SetActive(true);
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void SkillTakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        if (hpBar != null)
        {
            hpBar.SetHP(currentHP);
            hpBar.gameObject.SetActive(true);
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (hpBar != null)
            Destroy(hpBar.gameObject);

        // �ڱ� �ڽ� �±� Ȯ�� �� �׿� �´� Die �Լ� ȣ��
        if (CompareTag("Enemy"))
        {
            Enemy enemy = GetComponent<Enemy>();
            if (enemy != null) enemy.Die();
        }
        else if (CompareTag("DashEnemy"))
        {
            DashEnemy dashEnemy = GetComponent<DashEnemy>();
            if (dashEnemy != null) dashEnemy.Die();
        }
        else if (CompareTag("LongRangeEnemy"))
        {
            LongRangeEnemy longRangeEnemy = GetComponent<LongRangeEnemy>();
            if (longRangeEnemy != null) longRangeEnemy.Die();
        }
        else if (CompareTag("PotionEnemy"))
        {
            PotionEnemy potionEnemy = GetComponent<PotionEnemy>();
            if (potionEnemy != null) potionEnemy.Die();
        }

        //if (CompareTag("Enemy"))
        //{
        //    if (GameManager.Instance.enemy != null) GameManager.Instance.enemy.Die();
        //}
        //else if (CompareTag("DashEnemy"))
        //{
        //    if (GameManager.Instance.dashEnemy != null) GameManager.Instance.dashEnemy.Die();
        //}
        //else if (CompareTag("LongRangeEnemy"))
        //{
        //    if (GameManager.Instance.longRangeEnemy != null) GameManager.Instance.longRangeEnemy.Die();
        //}
        //else if (CompareTag("PotionEnemy"))
        //{
        //    if (GameManager.Instance.potionEnemy != null) GameManager.Instance.potionEnemy.Die();
        //}
    }
}
