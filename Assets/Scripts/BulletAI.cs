using UnityEngine;
using DG.Tweening;

public class BulletAI : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float followDuration = 0.3f;

    private Transform target;
    private Transform player;
    private bool isFollowingPlayer = true;
    private Coroutine moveCoroutine;
    private Collider2D myCollider;
    private Vector3 spawnOffset = Vector3.zero;

    public void SetSpawnOffset(Vector3 offset) { spawnOffset = offset; }

    public void SyncSetRotation(float angle)
    {
        if (isFollowingPlayer)
            transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnEnable()
    {
        transform.DOKill();
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
        StopAllCoroutines();

        isFollowingPlayer = true;
        target = null;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (myCollider == null)
            myCollider = GetComponent<Collider2D>();
        if (myCollider != null)
            myCollider.enabled = false;

        transform.localScale = Vector3.zero;

        // 위치, 회전은 BulletSpawner에서 지정(소환시마다)!
        transform.DOScale(0.5f, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            if (myCollider != null)
                myCollider.enabled = true;
            Invoke(nameof(SwitchToEnemy), followDuration);
        });
    }

    void Update()
    {
        if (isFollowingPlayer && player != null)
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
            transform.rotation = Quaternion.Euler(0, 0, angle);
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

    void ReturnToPoolSelf()
    {
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
