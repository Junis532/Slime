using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    [System.Serializable]
    public class Speaker
    {
        public Image imageDialog;
        public TextMeshProUGUI textDialog;
    }

    [System.Serializable]
    public struct DialogData
    {
        [TextArea(3, 5)]
        public string dialog;
        // public Sprite image; // 필요하면 다시 활성화
    }

    [Header("상점주인 대사들")]
    public List<DialogData> shopOwnerDialogs;

    public GameObject dialogPanel;
    public Speaker speakerUI;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 상점 입장 시 호출
    /// </summary>
    public void ShowRandomShopDialog()
    {
        if (shopOwnerDialogs.Count == 0)
        {
            Debug.LogWarning("상점 대사 데이터가 없습니다.");
            return;
        }

        int randomIndex = Random.Range(0, shopOwnerDialogs.Count);
        DialogData dialog = shopOwnerDialogs[randomIndex];

        Debug.Log($"[DialogManager] 선택된 대사: {dialog.dialog}");

        dialogPanel.SetActive(true);
        // 한 글자씩 타자 효과 없이 한 번에 표시
        speakerUI.textDialog.text = dialog.dialog;
    }
}
