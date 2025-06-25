using UnityEngine;

public class PlayerSize : MonoBehaviour
{
    private float originalScale;

    void Start()
    {
        // 초기 스케일 저장 (x 축 기준)
        originalScale = transform.localScale.x;
    }

    void Update()
    {
        float healthRatio = 1f;

        if (GameManager.Instance != null && GameManager.Instance.playerStats != null)
        {
            // CurrentHP와 MaxHP 사용 (대소문자 주의)
            healthRatio = GameManager.Instance.playerStats.currentHP / GameManager.Instance.playerStats.maxHP;
            healthRatio = Mathf.Clamp01(healthRatio);
        }

        // 체력 비율에 따라 크기 조절, 최소 크기 0.3 유지
        float newScale = Mathf.Max(originalScale * healthRatio, 0.7f);

        transform.localScale = new Vector3(newScale, newScale, transform.localScale.z);
    }
}
