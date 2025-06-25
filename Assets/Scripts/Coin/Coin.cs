using UnityEngine;

public class Coin : MonoBehaviour
{
    public float magnetRange = 3f;       // 자석 작동 거리
    public float moveSpeed = 10f;        // 코인이 빨려가는 속도

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

        // 범위 안에 들어오면 자동 흡수 시작
        if (!isAttracting && distance <= magnetRange)
        {
            isAttracting = true;
        }

        if (isAttracting)
        {
            // 플레이어 방향으로 이동
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            // 현재 거리 다시 계산
            distance = Vector3.Distance(transform.position, player.position);

            // 일정 거리 안에 들어오면 흡수
            if (distance < 0.3f)
            {
                CollectCoin();
            }
        }
    }

    // 외부에서 강제로 코인 흡수 시작
    public void AttractToPlayer()
    {
        isAttracting = true;
    }

    void CollectCoin()
    {
        // 코인 획득 로직 (게임 매니저에 코인 수 추가)
        GameManager.Instance.playerStats.coin += 1; // 또는 GameManager.Instance.AddCoin(1);
        Destroy(gameObject);
    }
}
