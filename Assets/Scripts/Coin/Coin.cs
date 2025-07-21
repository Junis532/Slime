using UnityEngine;

public class Coin : MonoBehaviour
{
    public float magnetRange = 3f;       // �ڼ� �۵� �Ÿ�
    public float moveSpeed = 10f;        // ������ �������� �ӵ�

    private Transform player;
    private bool isAttracting = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (!isAttracting && distance <= magnetRange)
        {
            isAttracting = true;
        }

        if (isAttracting)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CollectCoin();
        }
    }

    public void AttractToPlayer()
    {
        isAttracting = true;
    }

    void CollectCoin()
    {
        GameManager.Instance.playerStats.coin += 1;
        PoolManager.Instance.ReturnToPool(gameObject);
    }
}
