using UnityEngine;
using UnityEngine.UI;

public class FirstSkillImage : MonoBehaviour
{
    public Image skillSlot1;  // 1�� ��ų ���� UI (Image ������Ʈ)

    void Start()
    {
        if (SkillSelect.FinalSkillSprites != null && SkillSelect.FinalSkillSprites.Count >= 1)
        {
            skillSlot1.sprite = SkillSelect.FinalSkillSprites[0];
            skillSlot1.enabled = true;
        }
        else
        {
            Debug.LogWarning("����� ��ų �̹����� �����ϴ�.");
        }
    }
}
