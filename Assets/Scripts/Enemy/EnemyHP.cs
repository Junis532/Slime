using DG.Tweening;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public GameObject hpBarPrefab; // EnemyHPBar 프리팹 (PoolManager에 동일한 이름으로 등록 필요)
    private EnemyHPBar hpBar;
    private float currentHP;
    private float maxHP;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        maxHP = GameManager.Instance.enemyStats.maxHP;
        currentHP = maxHP;

        // 월드캔버스 찾아서 부모 지정
        Canvas worldCanvas = Object.FindAnyObjectByType<Canvas>();


        // 1. 풀에서 HP바 생성 (부모 설정)
        GameObject hpBarObj = PoolManager.Instance.SpawnFromPool(
            hpBarPrefab.name, Vector3.zero, Quaternion.identity);
        hpBarObj.transform.SetParent(worldCanvas.transform, false);

        // 2. 초기화 및 비활성화
        hpBar = hpBarObj.GetComponent<EnemyHPBar>();
        hpBar.Init(transform, maxHP);
        hpBarObj.SetActive(false);

        // 3. 스프라이트렌더러 캐싱
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer 컴포넌트를 찾지 못했습니다.");
        }
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

        PlayDamageEffect();

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

        PlayDamageEffect();

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void PlayDamageEffect()
    {
        if (spriteRenderer == null) return;

        // 빨간색으로 변경 후 0.2초 후 하얀색으로 복구
        spriteRenderer.DOColor(Color.red, 0.1f).OnComplete(() =>
        {
            spriteRenderer.DOColor(Color.white, 0.1f);
        });
    }

    private void Die()
    {
        // HP바 반환 (Destroy → ReturnToPool)
        if (hpBar != null)
            PoolManager.Instance.ReturnToPool(hpBar.gameObject);

        // 공통 EnemiesDie 스크립트 실행
        EnemiesDie enemiesDie = GetComponent<EnemiesDie>();
        if (enemiesDie != null)
        {
            enemiesDie.Die();
        }
    }
}
