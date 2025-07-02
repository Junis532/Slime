using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class GroundPrefabCreator : EditorWindow
{
    private Sprite spriteToConvert;
    private List<MonoScript> scriptsToAdd = new List<MonoScript>();
    private Vector2 scrollPos;
    private int selectedTagIndex = 0;
    private string[] tagOptions;

    [MenuItem("Tools/Ground Prefab Creator")]
    public static void ShowWindow()
    {
        GetWindow<GroundPrefabCreator>("Ground Prefab Creator");
    }

    private void OnEnable()
    {
        tagOptions = UnityEditorInternal.InternalEditorUtility.tags;
    }

    private void OnGUI()
    {
        GUILayout.Label("��������Ʈ �� Ground ������ ��ȯ", EditorStyles.boldLabel);

        spriteToConvert = (Sprite)EditorGUILayout.ObjectField("��������Ʈ", spriteToConvert, typeof(Sprite), false);

        GUILayout.Space(10);
        GUILayout.Label("�±� ����", EditorStyles.boldLabel);
        selectedTagIndex = EditorGUILayout.Popup("�±�", selectedTagIndex, tagOptions);

        GUILayout.Space(10);
        GUILayout.Label("�߰��� ��ũ��Ʈ ���", EditorStyles.boldLabel);

        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(100));
        for (int i = 0; i < scriptsToAdd.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            scriptsToAdd[i] = (MonoScript)EditorGUILayout.ObjectField(scriptsToAdd[i], typeof(MonoScript), false);
            if (GUILayout.Button("X", GUILayout.Width(25)))
                scriptsToAdd.RemoveAt(i);
            EditorGUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();

        if (GUILayout.Button("��ũ��Ʈ �߰�"))
        {
            scriptsToAdd.Add(null);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("������ ����"))
        {
            if (spriteToConvert == null)
            {
                Debug.LogError("��������Ʈ�� �������ּ���.");
                return;
            }

            CreateGroundPrefab(spriteToConvert, scriptsToAdd, tagOptions[selectedTagIndex]);
        }
    }

    private void CreateGroundPrefab(Sprite sprite, List<MonoScript> scripts, string tag)
    {
        GameObject go = new GameObject(sprite.name);
        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;

        BoxCollider2D bx = go.AddComponent<BoxCollider2D>();
        bx.isTrigger = true;
        

        // Rigidbody2D �߰� �� isTrigger ����
        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
        rb.simulated = true;
        rb.isKinematic = false;
        rb.useFullKinematicContacts = false;
        rb.sleepMode = RigidbodySleepMode2D.StartAwake;
        rb.interpolation = RigidbodyInterpolation2D.None;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        foreach (var script in scripts)
        {
            if (script == null) continue;
            var type = script.GetClass();
            if (type != null && type.IsSubclassOf(typeof(MonoBehaviour)))
                go.AddComponent(type);
        }

        if (!string.IsNullOrEmpty(tag)) go.tag = tag;

        string path = "Assets/Resources/GROUND/";
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        string prefabPath = $"{path}{sprite.name}.prefab";
        PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
        DestroyImmediate(go);

        Debug.Log($"������ ���� �Ϸ�: {prefabPath}");
    }
}
