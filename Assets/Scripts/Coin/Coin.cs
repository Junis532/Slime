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

        // ���� �ȿ� ������ �ڵ� ��� ����
        if (!isAttracting && distance <= magnetRange)
        {
            isAttracting = true;
        }

        if (isAttracting)
        {
            // �÷��̾� �������� �̵�
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            // ���� �Ÿ� �ٽ� ���
            distance = Vector3.Distance(transform.position, player.position);

            // ���� �Ÿ� �ȿ� ������ ���
            if (distance < 0.3f)
            {
                CollectCoin();
            }
        }
    }

    // �ܺο��� ������ ���� ��� ����
    public void AttractToPlayer()
    {
        isAttracting = true;
    }

    void CollectCoin()
    {
        // ���� ȹ�� ���� (���� �Ŵ����� ���� �� �߰�)
        GameManager.Instance.playerStats.coin += 1; // �Ǵ� GameManager.Instance.AddCoin(1);
        Destroy(gameObject);
    }
}
