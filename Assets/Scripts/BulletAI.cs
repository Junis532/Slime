using DG.Tweening;
using UnityEngine;

public class BulletAI : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float followDuration = 0.3f;
    public float baseSpawnOffsetDistance = 0.5f; // 기준 거리

    private float currentSpawnOffsetDistance;
    private float spawnOffsetVelocity = 0f;

    private Transform target;
    private Transform player;
    private bool isFollowingPlayer = true;

    private Vector3 spawnOffsetDirection; // 방향은 고정 (초기 랜덤 각도에서 뽑음)
    private Vector3 spawnOffset;

    private System.Random localRandom;
    private Collider2D myCollider;

    void Start()
    {
        transform.localScale = Vector3.zero;

        myCollider = GetComponent<Collider2D>();
        if (myCollider != null)
            myCollider.enabled = false;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogWarning("플레이어를 찾을 수 없습니다.");
            Destroy(gameObject);
            return;
        }

        localRandom = new System.Random(System.DateTime.Now.Millisecond + GetInstanceID());
        float angle = (float)(localRandom.NextDouble() * 360.0);
        float rad = angle * Mathf.Deg2Rad;
        spawnOffsetDirection = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f).normalized;

        // 초기 spawnOffset 거리 세팅
        currentSpawnOffsetDistance = baseSpawnOffsetDistance;
        spawnOffset = spawnOffsetDirection * currentSpawnOffsetDistance;

        transform.DOScale(0.5f, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            if (myCollider != null)
                myCollider.enabled = true;

            Invoke(nameof(SwitchToEnemy), followDuration);
        });
    }

    void Update()
    {
        if (player == null) return;

        float targetScale = Mathf.Max(Mathf.Abs(player.localScale.x), 1f);

        // 커지는 범위를 20% 정도만 반영하고 싶으면 0.2f 곱하기
        float scaleEffectFactor = 0.3f;

        currentSpawnOffsetDistance = Mathf.SmoothDamp(
            currentSpawnOffsetDistance,
            baseSpawnOffsetDistance * (1f + (targetScale - 1f) * scaleEffectFactor),
            ref spawnOffsetVelocity,
            0.3f);

        spawnOffset = spawnOffsetDirection * currentSpawnOffsetDistance;

        if (isFollowingPlayer)
        {
            transform.position = player.position + spawnOffset;
        }
    }



    void SwitchToEnemy()
    {
        isFollowingPlayer = false;
        FindClosestTarget();

        if (target != null)
        {
            StartCoroutine(MoveTowardsTarget());
        }
        else
        {
            Destroy(gameObject, 1f); // 적 없으면 1초 뒤 삭제
        }
    }

    System.Collections.IEnumerator MoveTowardsTarget()
    {
        while (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
            transform.position += direction * moveSpeed * Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    void FindClosestTarget()
    {
        string[] enemyTags = { "Enemy", "DashEnemy", "LongRangeEnemy", "PotionEnemy" };
        float closestDist = Mathf.Infinity;
        Transform closest = null;

        foreach (string tag in enemyTags)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject enemy in enemies)
            {
                float dist = Vector3.Distance(transform.position, enemy.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = enemy.transform;
                }
            }
        }

        target = closest;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("DashEnemy") ||
            other.CompareTag("LongRangeEnemy") || other.CompareTag("PotionEnemy"))
        {
            EnemyHP hp = other.GetComponent<EnemyHP>();
            if (hp != null)
            {
                hp.TakeDamage();
            }

            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        transform.DOKill(); // DOTween 메모리 정리
    }
}