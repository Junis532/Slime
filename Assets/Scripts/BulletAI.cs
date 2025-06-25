using DG.Tweening;
using UnityEngine;

public class BulletAI : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float followDuration = 0.3f; // �÷��̾ ����ٴϴ� �ð�
    public float followOffsetX = 0.5f;  // �÷��̾� �¿� ����ٴ� �Ÿ� ������

    private Transform target;
    private Transform player;
    private bool isFollowingPlayer = true;

    void Start()
    {
        transform.localScale = Vector3.zero;

        // �÷��̾� ã��
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogWarning("�÷��̾ ã�� �� �����ϴ�.");
            Destroy(gameObject);
            return;
        }

        // ���� �� Ŀ���� �ִϸ��̼�
        transform.DOScale(0.5f, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            // ���� �ð� �� ���� ���� ���ư�
            Invoke(nameof(SwitchToEnemy), followDuration);
        });
    }

    void Update()
    {
        if (isFollowingPlayer && player != null)
        {
            float direction = 1f;

            SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                direction = sr.flipX ? -1f : 1f;
            }
            else
            {
                direction = player.localScale.x < 0 ? -1f : 1f;
            }

            transform.position = player.position + new Vector3(followOffsetX * direction, 0f, 0f);
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
            Destroy(gameObject, 1f); // ���� ������ 1�� �� �ı�
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
        transform.DOKill(); // DOTween Ʈ�� ����
    }
}
