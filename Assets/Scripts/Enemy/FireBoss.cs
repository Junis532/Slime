using DG.Tweening;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    public float skillInterval = 4f;
    public float dropTime = 0.5f;
    public float stopDuration = 1f;

    private float skillTimer = 0f;
    private float stopTimer = 0f;
    private bool isDropping = false;
    private bool isStopping = false;
    private bool isSkillPlaying = false;   // 스킬 중엔 행동 금지
    private int currentSkillIndex;

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

    [Header("파이어볼 관련")]
    public GameObject fireballPrefab;
    public int numberOfFireballs = 36;

    [Header("파이어볼 경고 프리팹")]
    public GameObject fireballWarningPrefab;
    public float warningDuration = 1f;
    public float fireballSpawnRadius = 1.5f;

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

        // 스킬 모션 실행 중엔 내 행동 완전 금지!
        if (isSkillPlaying) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        Vector2 dirVec = (player.transform.position - transform.position);
        Vector2 inputVec = dirVec.normalized;

        // 드롭 중이면 아무 것도 안 함 (DOTween이 위치 갱신)
        if (isDropping)
        {
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Idle);
            return;
        }

        // 착지-정지(스킬 발동 대기) 상태
        if (isStopping)
        {
            stopTimer += Time.deltaTime;
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Idle);

            if (dashPreviewInstance != null)
            {
                Vector3 direction = new Vector3(inputVec.x, inputVec.y, 0f).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                dashPreviewInstance.transform.rotation = Quaternion.Euler(0f, 0f, angle);

                Vector3 basePos = transform.position + direction * previewDistanceFromEnemy;
                Vector3 offset = -dashPreviewInstance.transform.up * previewBackOffset;
                dashPreviewInstance.transform.position = basePos + offset;
            }

            if (stopTimer >= stopDuration)
            {
                stopTimer = 0f;
                isStopping = false;

                if (dashPreviewInstance != null)
                    dashPreviewInstance.SetActive(false);

                UseRandomSkill();
            }
            return;
        }

        // 이동 상태 ─ 주기 체크
        skillTimer += Time.deltaTime;
        if (skillTimer >= skillInterval)
        {
            skillTimer = 0f;
            if (!isSkillPlaying)      // 스킬 중, 드롭 재진입 금지
            {
                currentSkillIndex = Random.Range(0, 3);
                StartDropSequence();
            }
            return;
        }

        // 평상시 이동
        MoveTowards(inputVec);
    }

    // ────────── 드롭 시퀀스 (위→아래 모두 DOTween) ──────────
    private void StartDropSequence()
    {
        isDropping = true;

        Vector3 startPos = transform.position;
        Vector3 upPos = new Vector3(startPos.x, 20f, startPos.z);
        Vector3 tpDownPos = new Vector3(0f, 20f, 0f);
        Vector3 groundPos = new Vector3(0f, 0f, 0f);

        float upTime = dropTime * 0.6f;
        float downTime = dropTime * 0.4f;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(upPos, upTime).SetEase(Ease.OutQuad));
        seq.AppendCallback(() => transform.position = tpDownPos); // TP 후
        seq.Append(transform.DOMove(groundPos, downTime).SetEase(Ease.InQuad));
        seq.OnComplete(() =>
        {
            isDropping = false;
            isStopping = true;
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
        isSkillPlaying = true; // 스킬 도중엔 모든 Update 무효화

        switch (currentSkillIndex)
        {
            case 0:
                SkillPotion();
                break;
            case 1:
                StartCoroutine(SkillExplosionCoroutine());
                break;
            case 2:
                SkillDash();
                break;
        }
    }

    private void SkillPotion()
    {
        if (potionPrefab != null)
        {
            Instantiate(potionPrefab, transform.position, Quaternion.identity);
        }
        StartCoroutine(SkillEndDelay());
    }

    private IEnumerator SkillExplosionCoroutine()
    {
        Debug.Log("🔥 8방향 x 3 (발사 전 경고)");
        if (fireballPrefab == null)
        {
            StartCoroutine(SkillEndDelay());
            yield break;
        }
        int fireballCount = 8;
        float angleStep = 360f / fireballCount;
        Vector2 origin = transform.position;

        yield return StartCoroutine(FireballWarningAndBurst(origin, fireballCount, angleStep, 0f));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FireballWarningAndBurst(origin, fireballCount, angleStep, angleStep / 2));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FireballWarningAndBurst(origin, fireballCount, angleStep, angleStep / 3 + 30f));
        yield return new WaitForSeconds(1f);

        StartCoroutine(SkillEndDelay());
    }

    private IEnumerator FireballWarningAndBurst(Vector2 origin, int count, float angleStep, float angleOffset)
    {
        List<GameObject> warnings = new List<GameObject>();
        for (int i = 0; i < count; i++)
        {
            float angle = i * angleStep + angleOffset;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;
            Vector2 warnPos = origin + dir * fireballSpawnRadius;

            if (fireballWarningPrefab != null)
            {
                GameObject warn = Instantiate(fireballWarningPrefab, warnPos, Quaternion.Euler(0, 0, angle));
                warnings.Add(warn);
                Destroy(warn, warningDuration);
            }
        }
        yield return new WaitForSeconds(warningDuration);

        for (int i = 0; i < count; i++)
        {
            float angle = i * angleStep + angleOffset;
            FireInDirection(origin, angle);
        }
    }

    private void FireInDirection(Vector2 origin, float angle)
    {
        Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;
        GameObject fireball = Instantiate(fireballPrefab, origin, Quaternion.Euler(0, 0, angle));
        fireball.GetComponent<BossFireballProjectile>()?.Init(direction);
    }

    private void SkillDash()
    {
        Debug.Log("💨 Dash Skill!");
        // 예시: 실제 대시 연출 필요 시 여기에 코루틴도 가능
        StartCoroutine(SkillEndDelay());
    }

    private IEnumerator SkillEndDelay()
    {
        yield return new WaitForSeconds(1f);
        isSkillPlaying = false;
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
