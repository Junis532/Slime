using System.Collections;
using UnityEngine;

public class PoisonSpawner : MonoBehaviour
{
    public GameObject poisonPrefab;   // ���� �� ������
    public GameObject player;               // �÷��̾� ������Ʈ �Ҵ�

    public float spawnInterval = 15f;
    public float poisonLifetime = 10f;
    public Vector3 spawnOffset = Vector3.zero;  // �÷��̾� ��ġ ���� ������ (�� �� ������)

    private Coroutine spawnCoroutine;

    void Start()
    {
        if (poisonPrefab != null && player != null)
        {
            spawnCoroutine = StartCoroutine(SpawnPotionEnemyRoutine());
        }
        else
        {
            Debug.LogWarning("PotionEnemy ������ �Ǵ� Player ������Ʈ�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }

    private IEnumerator SpawnPotionEnemyRoutine()
    {
        while (true)
        {
            // �÷��̾� ���� ��ġ + ������ (�� �� ��ġ�� �°� ����)
            Vector3 spawnPos = player.transform.position + spawnOffset;

            GameObject potion = Instantiate(poisonPrefab, spawnPos, Quaternion.identity);

            // ������ �ʱ�ȭ
            PoisonDamage poisonDamage = potion.GetComponent<PoisonDamage>();
            if (poisonDamage != null)
            {
                poisonDamage.Init();
            }

            Destroy(potion, poisonLifetime);

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void OnDisable()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
    }
}
