using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 사용 시

public class PlayerHP : MonoBehaviour
{
    [Header("플레이어 체력바")]
    public Slider hpSlider;

    [Header("체력 숫자 표시 (예: 35 / 100)")]
    public TMP_Text hpText;  // 만약 Text라면 UnityEngine.UI.Text로 변경

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.playerStats != null)
        {
            float currentHP = GameManager.Instance.playerStats.currentHP;
            float maxHP = GameManager.Instance.playerStats.maxHP;

            // 슬라이더 최대값, 현재값 설정
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;

            // 텍스트 표시
            hpText.text = $"{(int)currentHP} / {(int)maxHP}";
        }
    }
}
