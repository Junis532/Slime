using System.Collections;
using UnityEngine;

public class PoisonSkill : MonoBehaviour
{
    public GameObject poisonPrefab;           // ������ �� ������
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

            GameObject poison = Instantiate(poisonPrefab, spawnPos, Quaternion.identity);

            // �ʱ�ȭ
            PoisonDamage poisonDamage = poison.GetComponent<PoisonDamage>();
            if (poisonDamage != null)
            {
                poisonDamage.Init();
            }

            Destroy(poison, poisonLifetime);

            yield return new WaitForSeconds(spawnInterval);
        }
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
