using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    [System.Serializable]
    public struct DialogData
    {
        [TextArea(3, 5)]
        public string dialog;
        // public Sprite image; // �ʿ�� ���
    }

    [System.Serializable]
    public class DialogPage
    {
        public List<DialogData> dialogs;  // �� �������� ���� �� ���
    }

    [Header("�������� ��� �������� (1������ = ���� ���)")]
    public List<DialogPage> shopDialogPages;

    public GameObject dialogPanel;
    public Image imageDialog;
    public TextMeshProUGUI textDialog;
    public Button nextDialogButton;

    private int currentPageIndex = 0;
    private int currentLineIndex = 0;

    private void Awake()
    {
        Instance = this;
        nextDialogButton.onClick.AddListener(NextLine);
    }

    public void StartShopDialog()
    {
        if (shopDialogPages.Count == 0)
        {
            Debug.LogWarning("���� ��� �������� �����ϴ�.");
            return;
        }

        // ���� ������ ����
        currentPageIndex = Random.Range(0, shopDialogPages.Count);
        currentLineIndex = 0;

        dialogPanel.SetActive(true);
        nextDialogButton.gameObject.SetActive(true);

        ShowCurrentLine();
    }

    void ShowCurrentLine()
    {
        DialogPage page = shopDialogPages[currentPageIndex];

        if (currentLineIndex >= page.dialogs.Count)
        {
            EndDialog();
            return;
        }

        textDialog.text = page.dialogs[currentLineIndex].dialog;
    }

    void NextLine()
    {
        currentLineIndex++;
        ShowCurrentLine();
    }

    void EndDialog()
    {
        //dialogPanel.SetActive(false);
        nextDialogButton.gameObject.SetActive(false);
        textDialog.text = string.Empty;
        imageDialog.gameObject.SetActive(false);
        Debug.Log("[DialogManager] ��ȭ ����");
    }

}
