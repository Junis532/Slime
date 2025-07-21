using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolInfo
{
    public string prefabName;
    public GameObject prefab;
    public int initialSize = 10;
}

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }
    public PoolInfo[] pools;
    private Dictionary<string, Queue<GameObject>> poolDict;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        poolDict = new Dictionary<string, Queue<GameObject>>();
        foreach (var pool in pools)
        {
            var queue = new Queue<GameObject>();
            for (int i = 0; i < pool.initialSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.name = pool.prefabName;  // �ڡڡ� �̸� ���� ����
                obj.SetActive(false);
                obj.transform.SetParent(transform);
                queue.Enqueue(obj);
            }
            poolDict.Add(pool.prefabName, queue);
        }
    }

    public GameObject SpawnFromPool(string prefabName, Vector3 pos, Quaternion rot)
    {
        if (!poolDict.ContainsKey(prefabName))
        {
            Debug.LogWarning($"{prefabName}�� �ش��ϴ� Ǯ ����!");
            return null;
        }
        GameObject obj = null;
        if (poolDict[prefabName].Count > 0)
        {
            obj = poolDict[prefabName].Dequeue();
        }
        else
        {
            var poolInfo = System.Array.Find(pools, x => x.prefabName == prefabName);
            if (poolInfo != null)
            {
                obj = Instantiate(poolInfo.prefab);
                obj.name = prefabName;
            }
        }
        if (obj == null) return null;

        obj.transform.SetParent(null);
        obj.transform.position = pos;
        obj.transform.rotation = rot;
        obj.SetActive(true);
        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);

        // �� �̸��� ���ų� �߸��Ǿ� ������ Destroy, �ƴϸ� Ǯ�� ��ȯ
        if (poolDict.ContainsKey(obj.name))
        {
            poolDict[obj.name].Enqueue(obj);
        }
        else
        {
            Debug.LogWarning($"[{obj.name}]��(��) Ǯ�� ��ϵ��� ���� ������Ʈ�� Destroy�մϴ�.");
            Destroy(obj);
        }
    }
}
