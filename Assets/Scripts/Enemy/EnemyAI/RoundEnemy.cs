using UnityEngine;

public class OrbitEnemy : EnemyBase
{
    private bool isLive = true;

    private SpriteRenderer spriter;
    private EnemyAnimation enemyAnimation;

    [Header("자기 중심 회전")]
    public float orbitRadius = 1.5f;         // 회전 반지름
    public float orbitSpeed = 120f;          // 회전 속도 (도/초)
    private float currentAngle = 0f;
    private Vector2 orbitCenter;             // 회전 중심

    void Start()
    {
        spriter = GetComponent<SpriteRenderer>();
        enemyAnimation = GetComponent<EnemyAnimation>();

        originalSpeed = GameManager.Instance.enemyStats.speed;
        speed = originalSpeed;

        orbitCenter = transform.position; // 회전 중심을 소환 위치로 고정
    }

    void Update()
    {
        if (!isLive) return;

        // 각도 업데이트
        currentAngle += orbitSpeed * Time.deltaTime;
        if (currentAngle >= 360f) currentAngle -= 360f;

        // 원운동 좌표 계산
        float rad = currentAngle * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * orbitRadius;
        transform.position = orbitCenter + offset;

        // 방향 반전 처리 (왼쪽/오른쪽)
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (offset.x < 0 ? -1 : 1);
        transform.localScale = scale;


        enemyAnimation.PlayAnimation(EnemyAnimation.State.Move);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isLive) return;

        if (collision.CompareTag("Player"))
        {
            int damage = GameManager.Instance.enemyStats.attack;
            GameManager.Instance.playerStats.currentHP -= damage;
            GameManager.Instance.playerDamaged.PlayDamageEffect();

            if (GameManager.Instance.playerStats.currentHP <= 0)
            {
                GameManager.Instance.playerStats.currentHP = 0;
                // 플레이어 사망 처리 필요 시 여기에
            }
        }
    }

    private void OnDestroy()
    {
        isLive = false;
    }
}
