using DG.Tweening;
using UnityEngine;

public class FireBoss : EnemyBase
{
    // ────────── 기본 데이터 ──────────
    private bool isLive = true;
    private SpriteRenderer spriter;
    private EnemyAnimation enemyAnimation;

    private Vector2 currentVelocity;
    private Vector2 currentDirection;
    public float smoothTime = 0.1f;

    // ────────── 스킬/타이밍 ──────────
    [Header("패턴 타이밍")]
    public float skillInterval = 4f;   // 이동하다가 떨어지기까지의 주기
    public float dropTime = 0.5f;  // y=10 → y=0 내려오는 시간
    public float stopDuration = 1f;    // 착지 후 멈춰 있는 시간

    private float skillTimer = 0f;      // 주기 카운터
    private float stopTimer = 0f;      // 멈춤 카운터
    private bool isDropping = false;   // 공중에서 내려오는 중?
    private bool isStopping = false;   // 착지 후 멈춰‑스킬 상태?
    private int currentSkillIndex;    // 0‧1‧2 중 랜덤

    // ────────── 시각 효과 ──────────
    [Header("범위 표시 프리팹")]
    public GameObject dashPreviewPrefab;
    public float previewDistanceFromEnemy = 0f;
    public float previewBackOffset = 0f;
    private GameObject dashPreviewInstance;

    // ────────── 예시 스킬(포션) ──────────
    [Header("포션 관련")]
    public GameObject potionPrefab;
    public float potionLifetime = 2f;

    // ────────── 초기화 ──────────
    void Start()
    {
        spriter = GetComponent<SpriteRenderer>();
        enemyAnimation = GetComponent<EnemyAnimation>();

        if (dashPreviewPrefab != null)
        {
            dashPreviewInstance = Instantiate(dashPreviewPrefab, transform.position, Quaternion.identity);
            dashPreviewInstance.SetActive(false);
        }

        originalSpeed = GameManager.Instance.potionEnemyStats.speed;
        speed = originalSpeed;
    }

    // ────────── 메인 루프 ──────────
    void Update()
    {
        if (!isLive) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        Vector2 dirVec = (player.transform.position - transform.position);
        Vector2 inputVec = dirVec.normalized;

        /* 1) 드롭 중이면 아무 것도 안 함 (DOTween이 위치 갱신) */
        if (isDropping)
        {
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Idle);
            return;
        }

        /* 2) 착지‑정지(스킬 발동) 상태 */
        if (isStopping)
        {
            stopTimer += Time.deltaTime;

            // 멈춰서는 동안 Idle
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Idle);

            // 범위 표시 따라다니게 할 경우
            if (dashPreviewInstance != null)
            {
                Vector3 direction = new Vector3(inputVec.x, inputVec.y, 0f).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                dashPreviewInstance.transform.rotation = Quaternion.Euler(0f, 0f, angle);

                Vector3 basePos = transform.position + direction * previewDistanceFromEnemy;
                Vector3 offset = -dashPreviewInstance.transform.up * previewBackOffset;
                dashPreviewInstance.transform.position = basePos + offset;
            }

            // 멈춤이 끝나면 이동 재개
            if (stopTimer >= stopDuration)
            {
                stopTimer = 0f;
                isStopping = false;

                if (dashPreviewInstance != null)
                    dashPreviewInstance.SetActive(false);
            }
            return;
        }

        /* 3) 이동 상태 ─ 주기 체크 */
        skillTimer += Time.deltaTime;
        if (skillTimer >= skillInterval)
        {
            skillTimer = 0f;
            currentSkillIndex = Random.Range(0, 3);   // 0,1,2 중 하나 선택
            StartDropSequence();                      // 드롭 시작
            return;
        }

        /* 4) 평상시 이동 */
        MoveTowards(inputVec);
    }

    // ────────── 드롭 시퀀스 ──────────
    private void StartDropSequence()
    {
        isDropping = true;

        // 즉시 상공으로 이동
        transform.position = new Vector3(0f, 10f, 0f);

        // y=0까지 내려오며 착지
        transform.DOMove(new Vector3(0f, 0f, 0f), dropTime)
                 .SetEase(Ease.InQuad)
                 .OnComplete(() =>
                 {
                     isDropping = false;
                     isStopping = true;   // 착지 후 멈춤‑스킬
                     UseRandomSkill();    // 멈춤 시작과 동시에 스킬 발동
                     stopTimer = 0f;

                     if (dashPreviewInstance != null)
                         dashPreviewInstance.SetActive(true);
                 });
    }

    // ────────── 이동 보조 ──────────
    private void MoveTowards(Vector2 inputVec)
    {
        currentDirection = Vector2.SmoothDamp(currentDirection, inputVec,
                                              ref currentVelocity, smoothTime);
        Vector2 nextVec = currentDirection * speed * Time.deltaTime;
        transform.Translate(nextVec);

        if (currentDirection.magnitude > 0.01f)
        {
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Move);
            FlipSprite(currentDirection.x);
        }
        else
        {
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Idle);
        }
    }

    // ────────── 랜덤 스킬 ──────────
    private void UseRandomSkill()
    {
        switch (currentSkillIndex)
        {
            case 0: SkillPotion(); break;
            case 1: SkillExplosion(); break;
            case 2: SkillDash(); break;
        }
    }

    private void SkillPotion()    // 예시 스킬 1
    {
        if (potionPrefab != null)
        {
            Instantiate(potionPrefab, transform.position, Quaternion.identity);
        }
    }

    private void SkillExplosion() // 예시 스킬 2
    {
        Debug.Log("⚡ Explosion Skill!");
        // TODO: 범위 데미지‧파티클
    }

    private void SkillDash()      // 예시 스킬 3
    {
        Debug.Log("💨 Dash Skill!");
        // TODO: 플레이어 향해 짧은 대시 등 구현
    }

    // ────────── 유틸 ──────────
    private void FlipSprite(float dirX)
    {
        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * (dirX < 0 ? -1 : 1);
        transform.localScale = s;
    }

    void OnDisable()
    {
        if (dashPreviewInstance != null)
            dashPreviewInstance.SetActive(false);
    }
    void OnDestroy()
    {
        if (dashPreviewInstance != null)
            Destroy(dashPreviewInstance);
    }
}
