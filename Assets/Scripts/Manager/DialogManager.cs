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
        // public Sprite image; // �ʿ��ϸ� �ٽ� Ȱ��ȭ
    }

    [Header("�������� ����")]
    public List<DialogData> shopOwnerDialogs;

    public GameObject dialogPanel;
    public Speaker speakerUI;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// ���� ���� �� ȣ��
    /// </summary>
    public void ShowRandomShopDialog()
    {
        if (shopOwnerDialogs.Count == 0)
        {
            Debug.LogWarning("���� ��� �����Ͱ� �����ϴ�.");
            return;
        }

        int randomIndex = Random.Range(0, shopOwnerDialogs.Count);
        DialogData dialog = shopOwnerDialogs[randomIndex];

        Debug.Log($"[DialogManager] ���õ� ���: {dialog.dialog}");

        dialogPanel.SetActive(true);
        // �� ���ھ� Ÿ�� ȿ�� ���� �� ���� ǥ��
        speakerUI.textDialog.text = dialog.dialog;
    }
}
