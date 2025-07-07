using UnityEngine;
using UnityEngine.UI;

public class FirstSkillImage : MonoBehaviour
{
    public Image skillSlot1;  // 1번 스킬 슬롯 UI (Image 컴포넌트)

    void Start()
    {
        if (SkillSelect.FinalSkillSprites != null && SkillSelect.FinalSkillSprites.Count >= 1)
        {
            skillSlot1.sprite = SkillSelect.FinalSkillSprites[0];
            skillSlot1.enabled = true;
        }
        else
        {
            Debug.LogWarning("저장된 스킬 이미지가 없습니다.");
        }
    }
}
