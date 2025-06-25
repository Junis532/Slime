using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class CSVSpawnGeneratorWindow : EditorWindow
{
    // 멤버 변수는 반드시 클래스 내부에 선언해야 합니다.
    private string csvFileName = "enemy_spawn_data";
    private string savePath = "Assets/Resources/EnemySpawnData";
    private MonsterDB monsterDB;
    private List<EnemySpawnDataPreview> previewList = new List<EnemySpawnDataPreview>();

    [MenuItem("Window/Tools/CSV 생성기")]
    public static void ShowWindow()
    {
        GetWindow<CSVSpawnGeneratorWindow>("CSV Spawn Generator");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("CSV 기반 스폰 데이터 생성기", EditorStyles.boldLabel);

        // 아래 두 변수는 모두 클래스 멤버여야 하며, 선언 안 했으면 반드시 선언 필요!
        csvFileName = EditorGUILayout.TextField("CSV 파일명", csvFileName);
        savePath = EditorGUILayout.TextField("저장 경로", savePath);

        GUILayout.Space(10);

        monsterDB = (MonsterDB)EditorGUILayout.ObjectField("Monster DB", monsterDB, typeof(MonsterDB), false);

        GUILayout.Space(10);

        if (GUILayout.Button("📄 CSV 미리보기"))
        {
            ParseCSV();
        }

        if (previewList.Count > 0)
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("🔍 미리보기", EditorStyles.boldLabel);

            using (var scroll = new GUILayout.ScrollViewScope(Vector2.zero, GUILayout.Height(200)))
            {
                foreach (var p in previewList)
                {
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.LabelField($"Enemy Index: {p.enemyIndex}");
                    EditorGUILayout.LabelField($"Spawner Count: {p.spawnerCount}");
                    EditorGUILayout.LabelField($"Min Spawn: {p.minSpawn}");
                    EditorGUILayout.LabelField($"Max Spawn: {p.maxSpawn}");
                    EditorGUILayout.EndVertical();
                }
            }

            GUILayout.Space(10);

            if (GUILayout.Button("🛠️ ScriptableObject 생성"))
            {
                CreateScriptableObjects();
            }

            if (GUILayout.Button("🧱 Spawner GameObjects 생성"))
            {
                CreateSpawnerObjects();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("CSV를 먼저 불러오세요.", MessageType.Info);
        }
    }

    private void CreateScriptableObjects()
    {
        if (previewList.Count == 0) return;

        Directory.CreateDirectory(savePath);

        for (int i = 0; i < previewList.Count; i++)
        {
            var p = previewList[i];
            EnemySpawnData data = ScriptableObject.CreateInstance<EnemySpawnData>();
            data.SpawnEnemyIndex = p.enemyIndex;
            data.SpawnerCount = p.spawnerCount;
            data.MinSpawn = p.minSpawn;
            data.MaxSpawn = p.maxSpawn;

            string assetPath = Path.Combine(savePath, $"EnemySpawnData_{i}.asset");
            AssetDatabase.CreateAsset(data, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("ScriptableObject 생성 완료!");
    }


    private void ParseCSV()
    {
        previewList.Clear();

        TextAsset csvData = Resources.Load<TextAsset>(csvFileName);
        if (csvData == null)
        {
            Debug.LogError($"CSV 파일을 찾을 수 없습니다: Resources/{csvFileName}.csv");
            return;
        }

        string[] lines = csvData.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)  // 헤더 제외
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Trim().Split(',');
            if (values.Length < 5) continue;

            if (int.TryParse(values[1], out int enemyIndex) &&
                int.TryParse(values[2], out int spawnerCount) &&
                int.TryParse(values[3], out int minSpawn) &&
                int.TryParse(values[4], out int maxSpawn))
            {
                previewList.Add(new EnemySpawnDataPreview
                {
                    enemyIndex = enemyIndex,
                    spawnerCount = spawnerCount,
                    minSpawn = minSpawn,
                    maxSpawn = maxSpawn
                });
            }
            else
            {
                Debug.LogWarning($"CSV 데이터 파싱 실패: {lines[i]}");
            }
        }

        Debug.Log($"CSV 파싱 완료 - {previewList.Count}개 항목");
    }

    private void CreateSpawnerObjects()
    {
        if (previewList.Count == 0)
        {
            Debug.LogWarning("먼저 CSV를 불러오세요.");
            return;
        }

        if (monsterDB == null || monsterDB.monsters == null)
        {
            Debug.LogError("MonsterDB ScriptableObject를 지정해주세요.");
            return;
        }

        GameObject masterGroup = new GameObject("AllWaves");
        Undo.RegisterCreatedObjectUndo(masterGroup, "Create AllWaves Group");

        for (int i = 0; i < previewList.Count; i++)
        {
            var p = previewList[i];
            GameObject waveGroup = new GameObject($"Wave_{i + 1}");
            waveGroup.transform.parent = masterGroup.transform;
            Undo.RegisterCreatedObjectUndo(waveGroup, $"Create Wave_{i + 1}");

            for (int j = 0; j < p.spawnerCount; j++)
            {
                GameObject spawner = new GameObject($"Spawner_{j + 1}");
                spawner.transform.parent = waveGroup.transform;
                Undo.RegisterCreatedObjectUndo(spawner, $"Create Spawner_{j + 1}");

                EnemySpawner enemySpawner = spawner.AddComponent<EnemySpawner>();

                // 초기화 및 프리팹 세팅
                if (enemySpawner.enemyPrefabs == null)
                    enemySpawner.enemyPrefabs = new List<GameObject>();
                else
                    enemySpawner.enemyPrefabs.Clear();

                if (p.enemyIndex >= 0 && p.enemyIndex < monsterDB.monsters.Count)
                {
                    enemySpawner.enemyPrefabs.Add(monsterDB.monsters[p.enemyIndex]);
                }
                else
                {
                    Debug.LogWarning($"MonsterDB에 존재하지 않는 인덱스: {p.enemyIndex}");
                }

                enemySpawner.minSpawnCount = p.minSpawn;
                enemySpawner.maxSpawnCount = p.maxSpawn;
            }
        }

        Debug.Log("Wave 그룹 및 스포너 생성 완료!");
    }

}

[System.Serializable]
public class EnemySpawnDataPreview
{
    public int enemyIndex;
    public int spawnerCount;
    public int minSpawn;
    public int maxSpawn;
}
