using DG.Tweening;
using System.Collections;
using UnityEngine;

public class BabyEnemy : EnemyBase
{
    private bool isLive = true;

    private SpriteRenderer spriter;
    private EnemyAnimation enemyAnimation;

    [Header("���� ����")]
    public GameObject spawnPrefab;          // ��ȯ�� ���� ������
    public float spawnInterval = 2f;        // ���� ����

    void Start()
    {
        spriter = GetComponent<SpriteRenderer>();
        enemyAnimation = GetComponent<EnemyAnimation>();

        originalSpeed = GameManager.Instance.enemyStats.speed;
        speed = originalSpeed;

        if (spawnPrefab != null)
        {
            StartCoroutine(SpawnLoop());
        }
    }

    IEnumerator SpawnLoop()
    {
        while (isLive)
        {
            SpawnMinion();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnMinion()
    {
        GameObject minion = Instantiate(spawnPrefab, transform.position, Quaternion.identity);
        minion.transform.localScale = spawnPrefab.transform.localScale * 0.5f; // ũ�� 0.5��� ���
    }

    private void OnDestroy()
    {
        isLive = false; // �ı� �� ���� �ߴ�
    }
}
