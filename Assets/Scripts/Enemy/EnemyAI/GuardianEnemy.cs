using DG.Tweening;
using System.Collections;
using UnityEngine;

public class GuardianEnemy : EnemyBase
{
    private bool isLive = true;

    private SpriteRenderer spriter;
    private EnemyAnimation enemyAnimation;

    private Vector2 currentVelocity;
    private Vector2 currentDirection;

    public float smoothTime = 0.1f;
    public float fireRange = 5f;

    private GameObject player;
    private LineRenderer laserLineRenderer;

    [Header("레이저 선 설정")]
    public Color laserColor = Color.red;
    public float laserWidth = 0.1f;

    // 데미지 타이머
    private bool isDamaging = false;

    void Start()
    {
        spriter = GetComponent<SpriteRenderer>();
        enemyAnimation = GetComponent<EnemyAnimation>();

        originalSpeed = GameManager.Instance.enemyStats.speed;
        speed = originalSpeed;

        player = GameObject.FindWithTag("Player");

        // LineRenderer 추가 및 설정
        laserLineRenderer = gameObject.AddComponent<LineRenderer>();
        laserLineRenderer.positionCount = 2;
        laserLineRenderer.enabled = false;
        laserLineRenderer.startWidth = laserWidth;
        laserLineRenderer.endWidth = laserWidth;

        // 커스텀 GlowLine 셰이더 적용
        Material laserMat = new Material(Shader.Find("Unlit/GlowLine"));
        laserMat.SetColor("_Color", laserColor);
        laserMat.SetColor("_EmissionColor", laserColor * 5f);  // 발광 강도 조절
        laserLineRenderer.material = laserMat;

        laserLineRenderer.startColor = laserColor;
        laserLineRenderer.endColor = laserColor;

        // 레이저가 스프라이트보다 위에 보이도록 정렬
        laserLineRenderer.sortingLayerName = "Default";
        laserLineRenderer.sortingOrder = 10;
    }

    void Update()
    {
        if (!isLive || player == null) return;

        Vector2 dirVec = (player.transform.position - transform.position);
        float distance = dirVec.magnitude;
        Vector2 inputVec = dirVec.normalized;

        // 움직임
        currentDirection = Vector2.SmoothDamp(currentDirection, inputVec, ref currentVelocity, smoothTime);
        Vector2 nextVec = currentDirection * speed * Time.deltaTime;
        transform.Translate(nextVec);

        // 방향 반전
        if (currentDirection.magnitude > 0.01f)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (currentDirection.x < 0 ? -1 : 1);
            transform.localScale = scale;

            enemyAnimation.PlayAnimation(EnemyAnimation.State.Move);
        }
        else
        {
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Idle);
        }

        // 레이저 및 데미지 처리
        if (distance <= fireRange)
        {
            if (!laserLineRenderer.enabled)
                laserLineRenderer.enabled = true;

            Vector3 startPos = transform.position;
            Vector3 endPos = player.transform.position;

            startPos.z = -1f; // 레이저가 플레이어와 적 스프라이트 위에 위치하도록 Z값 조절
            endPos.z = -1f;

            laserLineRenderer.SetPosition(0, startPos);
            laserLineRenderer.SetPosition(1, endPos);

            if (!isDamaging)
            {
                isDamaging = true;
                StartCoroutine(DealDamageRoutine());
            }
        }
        else
        {
            if (laserLineRenderer.enabled)
                laserLineRenderer.enabled = false;

            if (isDamaging)
            {
                isDamaging = false;
                StopAllCoroutines();
            }
        }
    }

    private IEnumerator DealDamageRoutine()
    {
        while (isDamaging)
        {
            if (player == null) yield break;
            yield return new WaitForSeconds(1f);

            var playerStats = GameManager.Instance.playerStats;
            playerStats.currentHP -= 1;
            GameManager.Instance.playerDamaged.PlayDamageEffect();

            if (playerStats.currentHP <= 0)
            {
                playerStats.currentHP = 0;
                // 플레이어 죽음 처리 가능
            }
        }
    }
}
