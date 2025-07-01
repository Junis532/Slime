using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [Header("🔫 총알 프리팹")]
    public GameObject bulletPrefab;

    [Header("🕒 전체 생성 간격")]
    public float spawnInterval = 2f;

    [Header("📏 플레이어 주변 반경")]
    public float spawnOffset = 0.5f;

    [Header("🎯 동시에 생성할 총알 개수")]
    public int bulletCount = 3;

    private float timer;
    private System.Random localRandom;

    void Awake()
    {
        int seed = System.DateTime.Now.Millisecond + GetInstanceID();
        localRandom = new System.Random(seed);
    }

    void Update()
    {
        if (!GameManager.Instance.IsGame())
            return;

        // 현재 씬에 적 있는지 확인
        bool hasEnemy = false;
        string[] enemyTags = { "Enemy", "DashEnemy", "LongRangeEnemy", "PotionEnemy" };

        foreach (string tag in enemyTags)
        {
            if (GameObject.FindGameObjectWithTag(tag) != null)
            {
                hasEnemy = true;
                break;
            }
        }

        if (!hasEnemy)
            return;  // 적 없으면 아예 스폰 안 함

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null || bulletPrefab == null)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            Vector3 playerPos = player.transform.position;

            List<Vector3> spawnedPositions = new List<Vector3>();

            for (int i = 0; i < bulletCount; i++)
            {
                Vector3 spawnPos = GetNonOverlappingPosition(playerPos, spawnedPositions);
                Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
                spawnedPositions.Add(spawnPos);
            }

            timer = 0f;
        }
    }


    Vector3 GetNonOverlappingPosition(Vector3 centerPos, List<Vector3> existingPositions)
    {
        const int maxAttempts = 10;
        float minDistance = spawnOffset * 0.5f;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            float angle = (float)(localRandom.NextDouble() * 360.0);
            float rad = angle * Mathf.Deg2Rad;
            Vector3 randomDir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);
            Vector3 candidatePos = centerPos + randomDir * spawnOffset;

            bool overlaps = false;
            foreach (var pos in existingPositions)
            {
                if (Vector3.Distance(candidatePos, pos) < minDistance)
                {
                    overlaps = true;
                    break;
                }
            }

            if (!overlaps)
                return candidatePos;
        }

        return centerPos;
    }
}
