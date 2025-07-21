using DG.Tweening;
using UnityEngine;

public class BulletAI : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float followDuration = 0.3f;
    public float baseSpawnOffsetDistance = 0.5f;

    private float currentSpawnOffsetDistance;
    private float spawnOffsetVelocity = 0f;

    private Transform target;
    private Transform player;
    private bool isFollowingPlayer = true;

    private Vector3 spawnOffsetDirection;
    private Vector3 spawnOffset;

    private System.Random localRandom;
    private Collider2D myCollider;
    private Coroutine moveCoroutine;

    private GroupController groupController;

    void OnEnable()
    {
        // 1. 트윈/코루틴 확실히 종료
        transform.DOKill();
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
        StopAllCoroutines();

        // 2. 주요 변수 리셋
        isFollowingPlayer = true;
        target = null;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (myCollider == null)
            myCollider = GetComponent<Collider2D>();
        if (myCollider != null)
            myCollider.enabled = false; // (초기화!)

        transform.localScale = Vector3.zero;

        // 3. 스폰 각도/오프셋 새 랜덤 세팅
        localRandom = new System.Random(System.DateTime.Now.Millisecond + GetInstanceID());
        float angle = (float)(localRandom.NextDouble() * 360.0);
        float rad = angle * Mathf.Deg2Rad;
        spawnOffsetDirection = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f).normalized;

        currentSpawnOffsetDistance = baseSpawnOffsetDistance;
        spawnOffset = spawnOffsetDirection * currentSpawnOffsetDistance;

        // 4. 위치/회전 초기화
        if (player != null)
            transform.position = player.position + spawnOffset;
        else
            transform.position = Vector3.zero;

        transform.rotation = Quaternion.identity;

        // 5. 스폰 트윈 + 콜라이더 활성 예약 + 타겟 전환 예약
        transform.DOScale(0.5f, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            if (myCollider != null)
                myCollider.enabled = true;

            Invoke(nameof(SwitchToEnemy), followDuration);
        });
    }

    void OnDisable()
    {
        // 1. 트윈/코루틴 확실히 종료
        transform.DOKill();
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
        StopAllCoroutines();

        // 2. 콜라이더도 비활성화 (혹시 남아있을 수 있음)
        if (myCollider != null)
            myCollider.enabled = false;
    }

    void Update()
    {
        if (player == null) return;

        float targetScale = Mathf.Max(Mathf.Abs(player.localScale.x), 1f);
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
            moveCoroutine = StartCoroutine(MoveTowardsTarget());
        }
        else
        {
            ReturnToPoolSelf();
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

        ReturnToPoolSelf();
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
                hp.TakeDamage();

            ReturnToPoolSelf();
        }
    }

    // 풀 매니저로 반환하면서도 내부 동작 100% 정리(반복안되게)
    void ReturnToPoolSelf()
    {
        // 보장차원에서 추가, Disable 전에 시행
        transform.DOKill();
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
        StopAllCoroutines();

        if (myCollider != null)
            myCollider.enabled = false;

        PoolManager.Instance.ReturnToPool(gameObject);
    }
}
