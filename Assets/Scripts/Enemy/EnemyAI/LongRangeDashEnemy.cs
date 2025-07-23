using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeDashEnemy : EnemyBase
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

    [Header("벽 레이어 마스크")]
    public LayerMask wallLayerMask;

    [Header("대시 중 총알 발사 설정")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 3f;
    public float bulletLifetime = 2f;

    public int bulletsPerSide = 3;         // 좌우 각각 총알 개수
    public float sideBulletAngleStep = 10f; // 좌우 총알 사이 각도 간격

    public float dashFireCooldown = 0.1f;  // 대시 중 총알 발사 간격(초)
    private float lastDashFireTime = 0f;   // 마지막 대시 중 발사 시간

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
                lastDashFireTime = 0f; // 대시 시작 시 발사 타이머 초기화
                if (dashPreviewInstance != null)
                    dashPreviewInstance.SetActive(false);
            }
            return;
        }

        dashTimer += Time.deltaTime;
        if (dashTimer >= dashCooldown)
        {
            isPreparingToDash = true;
            pauseTimer = 0f;
            dashDirection = inputVec;
            return;
        }

        currentDirection = Vector2.SmoothDamp(currentDirection, inputVec, ref currentVelocity, smoothTime);
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

            // 대시 중 총알 발사 쿨다운 체크
            if (Time.time - lastDashFireTime >= dashFireCooldown)
            {
                FireBulletsSideways(dashDirection);
                lastDashFireTime = Time.time;
            }
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
    }

    // 대시 방향 기준으로 양 옆으로 총알을 좌우 각각 bulletsPerSide 개수씩 발사
    private void FireBulletsSideways(Vector2 centerDirection)
    {
        // 오른쪽 기준 수직 방향 벡터 (centerDirection에서 90도 회전)
        Vector2 rightNormal = new Vector2(centerDirection.y, -centerDirection.x).normalized;

        // 오른쪽 총알 발사 (양의 각도)
        for (int i = 1; i <= bulletsPerSide; i++)
        {
            float angle = sideBulletAngleStep * i;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * centerDirection;
            SpawnBullet(dir);
        }

        // 왼쪽 총알 발사 (음의 각도)
        for (int i = 1; i <= bulletsPerSide; i++)
        {
            float angle = -sideBulletAngleStep * i;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * centerDirection;
            SpawnBullet(dir);
        }
    }

    private void SpawnBullet(Vector2 direction)
    {
        GameObject bullet = PoolManager.Instance.SpawnFromPool(bulletPrefab.name, transform.position, Quaternion.identity);
        if (bullet != null)
        {
            BulletBehavior bulletBehavior = bullet.GetComponent<BulletBehavior>();
            if (bulletBehavior == null)
                bulletBehavior = bullet.AddComponent<BulletBehavior>();

            bulletBehavior.Initialize(direction.normalized, bulletSpeed, bulletLifetime);
        }
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
    }
}
