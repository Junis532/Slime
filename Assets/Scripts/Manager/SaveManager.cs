//using UnityEngine;
//using System.IO;

//public class SaveManager : MonoSigleTone<SaveManager>
//{
//    string filePath = Application.dataPath + "/playerData.txt";


//    private void Start()
//    {
//        Debug.Log(Application.dataPath);
//    }
//    public void SaveData(PlayerData data)
//    {
//        string json = JsonUtility.ToJson(data, true); // true�� ���� ���� ������
//        File.WriteAllText(filePath, json);
//        Debug.Log("���� �Ϸ�: " + filePath);
//    }

//    public PlayerData LoadData()
//    {
//        if (File.Exists(filePath))
//        {
//            Directory.CreateDirectory(filePath);
//            Directory.Delete(filePath, true);
//            string json = File.ReadAllText(filePath);
//            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
//            Debug.Log("�ҷ����� �Ϸ�");
//            return data;
//        }
//        else
//        {
//            Debug.LogWarning("���� ����");
//            return null;
//        }
//    }
//}
