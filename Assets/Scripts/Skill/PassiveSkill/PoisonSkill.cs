using System.Collections;
using UnityEngine;

public class PoisonSkill : MonoBehaviour
{
    public GameObject poisonPrefab;           // ������ �� ������ (PoolManager�� ��� �ʿ�)
    public float spawnInterval = 15f;         // ���� ����
    public float poisonLifetime = 10f;        // �� ���� �ð�
    public Vector3 spawnOffset = Vector3.zero; // �� �� ��ġ ����

    private Coroutine spawnCoroutine;

    void Start()
    {
        if (poisonPrefab != null)
        {
            spawnCoroutine = StartCoroutine(SpawnPoisonRoutine());
        }
        else
        {
            Debug.LogWarning("poisonPrefab�� �������� �ʾҽ��ϴ�.");
        }
    }

    private IEnumerator SpawnPoisonRoutine()
    {
        while (true)
        {
            Vector3 spawnPos = transform.position + spawnOffset;

            // Instantiate �� PoolManager ���
            GameObject poison = PoolManager.Instance.SpawnFromPool(poisonPrefab.name, spawnPos, Quaternion.identity);

            // �ʱ�ȭ
            PoisonDamage poisonDamage = poison.GetComponent<PoisonDamage>();
            if (poisonDamage != null)
            {
                poisonDamage.Init();
            }

            // Destroy �� ���� �ð� �� ReturnToPool
            StartCoroutine(ReturnPoisonToPool(poison, poisonLifetime));

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private IEnumerator ReturnPoisonToPool(GameObject poison, float time)
    {
        yield return new WaitForSeconds(time);
        PoolManager.Instance.ReturnToPool(poison);
    }

    void OnDisable()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }
}
