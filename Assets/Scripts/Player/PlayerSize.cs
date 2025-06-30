using UnityEngine;

public class PlayerSize : MonoBehaviour
{
    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.playerStats == null) return;

        float currentHP = GameManager.Instance.playerStats.currentHP;

        float scaleFactor = 0.3f;  // �⺻ ü�� 10�� �� ũ�� 3�� ����� ���� ���

        // �ּ� ũ�� 1 ����
        float newScale = Mathf.Max(1f, currentHP * scaleFactor);

        transform.localScale = new Vector3(newScale, newScale, transform.localScale.z);
    }
}
