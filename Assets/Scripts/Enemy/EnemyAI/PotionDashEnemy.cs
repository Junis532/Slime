using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionDashEnemy : EnemyBase
{
    private bool isLive = true;

    private SpriteRenderer spriter;
    private EnemyAnimation enemyAnimation;

    private Vector2 currentVelocity;
    private Vector2 currentDirection;

    public float smoothTime = 0.1f;
    public float dashSpeed = 20f;
    public float dashCooldown = 3f;
    public float pauseBeforeDash = 0.3f;
    public float dashDuration = 0.3f;

    private float dashTimer = 0f;
    private float dashTimeElapsed = 0f;
    private float pauseTimer = 0f;

    private bool isPreparingToDash = false;
    private bool isDashing = false;
    private bool isPausingAfterDash = false;
    private Vector2 dashDirection;
    private Vector2 dashEndPosition;

    [Header("대시 경로 프리뷰")]
    public GameObject dashPreviewPrefab;
    public float previewDistanceFromEnemy = 0f;
    public float previewBackOffset = 0f;
    private GameObject dashPreviewInstance;

    [Header("포션 생성 경고 프리뷰")]
    public GameObject potionWarningPrefab;
    public float potionWarningOffset = 0f;
    public float potionWarningDuration = 1f;
    private List<Vector3> warningPositions = new List<Vector3>();
    private List<GameObject> warningInstances = new List<GameObject>();

    [Header("벽 레이어 마스크")]
    public LayerMask wallLayerMask;

    [Header("대시 후 포션 생성 설정")]
    public GameObject potionDamagePrefab;
    public float potionLifetime = 2f;
    public float potionSpreadRadius = 1f;
    public int numberOfPotionsToSpawn = 6;

    [Header("회피 관련")]
    public float avoidanceRange = 1.5f;
    public LayerMask obstacleMask;  // 장애물 레이어 (Wall 등)

    void Start()
    {
        spriter = GetComponent<SpriteRenderer>();
        enemyAnimation = GetComponent<EnemyAnimation>();

        originalSpeed = GameManager.Instance.dashEnemyStats.speed;
        speed = originalSpeed;

        if (dashPreviewPrefab != null)
        {
            dashPreviewInstance = Instantiate(dashPreviewPrefab, transform.position, Quaternion.identity);
            dashPreviewInstance.SetActive(false);
        }
    }

    void Update()
    {
        if (!isLive) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        Vector2 currentPos = transform.position;
        Vector2 dirVec = (player.transform.position - transform.position);
        Vector2 inputVec = dirVec.normalized;

        if (isPausingAfterDash) return;

        if (isDashing)
        {
            DashMove();
            dashTimeElapsed += Time.deltaTime;

            enemyAnimation.PlayAnimation(EnemyAnimation.State.Move);
            FlipSprite(dashDirection.x);

            if (dashTimeElapsed >= dashDuration)
                EndDash();

            if (dashPreviewInstance != null)
                dashPreviewInstance.SetActive(false);

            return;
        }

        if (isPreparingToDash)
        {
            pauseTimer += Time.deltaTime;
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Idle);

            if (dashPreviewInstance != null)
            {
                dashPreviewInstance.SetActive(true);

                Vector3 direction = new Vector3(dashDirection.x, dashDirection.y, 0f).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                dashPreviewInstance.transform.rotation = Quaternion.Euler(0f, 0f, angle);

                Vector3 basePos = transform.position + direction * previewDistanceFromEnemy;
                Vector3 offset = -dashPreviewInstance.transform.up * previewBackOffset;
                dashPreviewInstance.transform.position = basePos + offset;
            }

            if (pauseTimer >= pauseBeforeDash)
            {
                isPreparingToDash = false;
                isDashing = true;
                pauseTimer = 0f;
                if (dashPreviewInstance != null)
                    dashPreviewInstance.SetActive(false);
            }
            return;
        }

        // **회피 기능 추가 (대시 중이 아닐 때만)**
        Vector2 avoidanceVec = Vector2.zero;
        RaycastHit2D hitAvoid = Physics2D.Raycast(currentPos, inputVec, avoidanceRange, obstacleMask);
        if (hitAvoid.collider != null)
        {
            Vector2 hitNormal = hitAvoid.normal;
            Vector2 sideStep = Vector2.Perpendicular(hitNormal).normalized;
            avoidanceVec = sideStep * 1.5f;
            Debug.DrawRay(currentPos, sideStep * 2, Color.green);
        }

        Vector2 finalMoveDir = (inputVec + avoidanceVec).normalized;

        currentDirection = Vector2.SmoothDamp(currentDirection, finalMoveDir, ref currentVelocity, smoothTime);
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

        if (dashPreviewInstance != null)
            dashPreviewInstance.SetActive(false);

        dashTimer += Time.deltaTime;
        if (dashTimer >= dashCooldown)
        {
            isPreparingToDash = true;
            pauseTimer = 0f;
            dashDirection = inputVec;
            return;
        }
    }

    private void DashMove()
    {
        Vector2 moveVec = dashDirection * dashSpeed * Time.deltaTime;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dashDirection, moveVec.magnitude, wallLayerMask);

        if (hit.collider != null)
        {
            transform.position = hit.point - dashDirection.normalized * 0.01f;
            EndDash();
        }
        else
        {
            transform.Translate(moveVec);
        }
    }

    private void EndDash()
    {
        isDashing = false;
        dashTimeElapsed = 0f;
        dashTimer = 0f;

        dashEndPosition = transform.position;

        currentDirection = Vector2.zero;
        currentVelocity = Vector2.zero;

        StartCoroutine(ShowPotionWarningsThenSpawn());
    }

    private IEnumerator ShowPotionWarningsThenSpawn()
    {
        isPausingAfterDash = true;

        SpawnPotionWarnings(dashEndPosition);

        yield return new WaitForSeconds(potionWarningDuration);

        SpawnPotionsAtWarnings();
        ClearPotionWarnings();

        isPausingAfterDash = false;
    }

    private void SpawnPotionWarnings(Vector2 centerPosition)
    {
        ClearPotionWarnings();

        float angleStep = 360f / numberOfPotionsToSpawn;

        for (int i = 0; i < numberOfPotionsToSpawn; i++)
        {
            float currentAngle = i * angleStep;
            Vector2 direction = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));
            Vector3 warningPos = centerPosition + direction * potionSpreadRadius + new Vector2(0, potionWarningOffset);

            GameObject warning = Instantiate(potionWarningPrefab, warningPos, Quaternion.identity);
            warning.SetActive(true);

            warningInstances.Add(warning);
            warningPositions.Add(warningPos);
        }
    }

    private void ClearPotionWarnings()
    {
        foreach (var go in warningInstances)
        {
            if (go != null)
                Destroy(go);
        }
        warningInstances.Clear();
        warningPositions.Clear();
    }

    private void SpawnPotionsAtWarnings()
    {
        foreach (Vector3 pos in warningPositions)
        {
            GameObject potion = Instantiate(potionDamagePrefab, pos, Quaternion.identity);

            if (potion.TryGetComponent(out PotionBehavior pb))
                pb.StartLifetime(potionLifetime);
        }

        Debug.Log($"{warningPositions.Count}개 포션 생성 완료.");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isLive) return;

        if (collision.CompareTag("Player"))
        {
            int damage = GameManager.Instance.dashEnemyStats.attack;
            GameManager.Instance.playerStats.currentHP -= damage;
            GameManager.Instance.playerDamaged.PlayDamageEffect();

            if (GameManager.Instance.playerStats.currentHP <= 0)
            {
                GameManager.Instance.playerStats.currentHP = 0;
                // 플레이어 죽음 처리
            }
        }
    }

    public void Knockback(Vector2 force)
    {
        if (isPreparingToDash || isDashing || isPausingAfterDash)
        {
            Debug.Log("대쉬 상태 중이라 넉백 무시");
            return;
        }

        transform.position += (Vector3)force;
    }

    private void FlipSprite(float directionX)
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (directionX < 0 ? -1 : 1);
        transform.localScale = scale;
    }

    void OnDisable()
    {
        if (dashPreviewInstance != null)
            dashPreviewInstance.SetActive(false);

        ClearPotionWarnings();

        StopAllCoroutines();
        isPreparingToDash = false;
        isDashing = false;
        isPausingAfterDash = false;
        dashTimer = 0f;
        pauseTimer = 0f;
    }

    void OnDestroy()
    {
        if (dashPreviewInstance != null)
            Destroy(dashPreviewInstance);

        ClearPotionWarnings();
    }
}
