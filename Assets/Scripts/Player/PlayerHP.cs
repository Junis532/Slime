using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro ��� ��

public class PlayerHP : MonoBehaviour
{
    [Header("�÷��̾� ü�¹�")]
    public Slider hpSlider;

    [Header("ü�� ���� ǥ�� (��: 35 / 100)")]
    public TMP_Text hpText;  // ���� Text��� UnityEngine.UI.Text�� ����

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.playerStats != null)
        {
            float currentHP = GameManager.Instance.playerStats.currentHP;
            float maxHP = GameManager.Instance.playerStats.maxHP;

            // �����̴� �ִ밪, ���簪 ����
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;

            // �ؽ�Ʈ ǥ��
            hpText.text = $"{(int)currentHP} / {(int)maxHP}";
        }
    }
}
