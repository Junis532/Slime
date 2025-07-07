using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillSelect : MonoBehaviour
{
    public Image[] bottomSlots; // 하단 선택 슬롯 (8개)
    public Image[] topSlots;    // 상단 배치 슬롯 (4개)
    public Sprite[] skillSprites; // 스킬 이미지 (1~4번)
    public Image[] resetSlots;
    public Button resetButton;
    public Button confirmButton;

    private List<int> selectedIndices = new(); // 선택된 순서
    private int currentIndex = 0;

    void OnEnable()
    {
        Time.timeScale = 0f;

        resetButton.onClick.AddListener(ResetSelection);
        confirmButton.onClick.AddListener(ConfirmSelection);

        for (int i = 0; i < bottomSlots.Length; i++)
        {
            int index = i;
            bottomSlots[i].GetComponent<Button>().onClick.AddListener(() => SelectSkill(index));
        }

        ResetSelection();
    }

    void OnDisable()
    {
        Time.timeScale = 1f;
    }


    void SelectSkill(int index)
    {
        if (currentIndex >= topSlots.Length || selectedIndices.Contains(index))
            return;

        topSlots[currentIndex].sprite = bottomSlots[index].sprite;
        selectedIndices.Add(index);
        currentIndex++;
    }

    void ResetSelection()
    {
        for (int i = 0; i < topSlots.Length; i++)
        {
            if (i < resetSlots.Length && resetSlots[i] != null)
                topSlots[i].sprite = resetSlots[i].sprite;  // resetSlots에 있는 이미지로 초기화
            else
                topSlots[i].sprite = null;
        }

        selectedIndices.Clear();
        currentIndex = 0;
    }


    void ConfirmSelection()
    {
        // 예시: 실제 게임에 사용할 순서 저장
        Debug.Log("저장된 스킬 인덱스:");
        foreach (var idx in selectedIndices)
            Debug.Log(idx);

        // 예: GameManager.Instance.SetSkillOrder(selectedIndices);
    }

    // 주사위 결과 → UI 반영
    public void ApplyDiceResult(int result)
    {
        if (result >= 1 && result <= 4 && skillSprites.Length >= result)
        {
            // 하단 슬롯에 자동 적용
            bottomSlots[result - 1].sprite = skillSprites[result - 1];
        }
    }
}
