using UnityEngine;

public class PlayerSize : MonoBehaviour
{
    private float originalScale;

    void Start()
    {
        // �ʱ� ������ ���� (x �� ����)
        originalScale = transform.localScale.x;
    }

    void Update()
    {
        float healthRatio = 1f;

        if (GameManager.Instance != null && GameManager.Instance.playerStats != null)
        {
            // CurrentHP�� MaxHP ��� (��ҹ��� ����)
            healthRatio = GameManager.Instance.playerStats.currentHP / GameManager.Instance.playerStats.maxHP;
            healthRatio = Mathf.Clamp01(healthRatio);
        }

        // ü�� ������ ���� ũ�� ����, �ּ� ũ�� 0.3 ����
        float newScale = Mathf.Max(originalScale * healthRatio, 0.7f);

        transform.localScale = new Vector3(newScale, newScale, transform.localScale.z);
    }
}
