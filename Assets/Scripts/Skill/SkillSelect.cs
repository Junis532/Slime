using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillSelect : MonoBehaviour
{
    public Image[] bottomSlots; // �ϴ� ���� ���� (8��)
    public Image[] topSlots;    // ��� ��ġ ���� (4��)
    public Sprite[] skillSprites; // ��ų �̹��� (1~4��)
    public Image[] resetSlots;
    public Button resetButton;
    public Button confirmButton;

    private List<int> selectedIndices = new(); // ���õ� ����
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
                topSlots[i].sprite = resetSlots[i].sprite;  // resetSlots�� �ִ� �̹����� �ʱ�ȭ
            else
                topSlots[i].sprite = null;
        }

        selectedIndices.Clear();
        currentIndex = 0;
    }


    void ConfirmSelection()
    {
        // ����: ���� ���ӿ� ����� ���� ����
        Debug.Log("����� ��ų �ε���:");
        foreach (var idx in selectedIndices)
            Debug.Log(idx);

        // ��: GameManager.Instance.SetSkillOrder(selectedIndices);
    }

    // �ֻ��� ��� �� UI �ݿ�
    public void ApplyDiceResult(int result)
    {
        if (result >= 1 && result <= 4 && skillSprites.Length >= result)
        {
            // �ϴ� ���Կ� �ڵ� ����
            bottomSlots[result - 1].sprite = skillSprites[result - 1];
        }
    }
}
