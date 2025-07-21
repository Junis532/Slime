using DG.Tweening;
using System.Collections;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [Header("🔫 총알 프리팹")]
    public GameObject bulletPrefab;

    [Header("🕒 전체 생성 간격")]
    public float spawnInterval = 2f;

    [Header("🎯 동시에 생성할 총알 개수")]
    public int bulletCount = 1;

    [Header("🏹 플레이어 따라다니는 활 프리팹")]
    public GameObject bowPrefab;
    [Header("🌟 화살 발사 연출용 효과 활 프리팹")]
    public GameObject effectBowPrefab;

    [Header("↖ 활이 따라올 오프셋(Vector3)")]
    public Vector3 bowOffset = new Vector3(0, -0.6f, 0);

    private float timer;
    private GameObject bowInstance;
    private GameObject effectBowInstance;
    private Transform playerTransform;
    private bool isBowActive = true;
    private BulletAI lastArrowAI = null; // 최근 발사한(준비중인) 화살
    private bool arrowIsFlying = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;

        if (bowPrefab != null)
        {
            bowInstance = Instantiate(bowPrefab);
            bowInstance.SetActive(true);
        }
        if (effectBowPrefab != null)
        {
            effectBowInstance = Instantiate(effectBowPrefab);
            effectBowInstance.SetActive(false);
        }
    }

    void Update()
    {
        if (isBowActive)
            UpdateBowFollowPlayer();

        if (!GameManager.Instance.IsGame())
            return;

        // 적 없는 경우 소환X
        bool hasEnemy = false;
        string[] enemyTags = { "Enemy", "DashEnemy", "LongRangeEnemy", "PotionEnemy" };
        foreach (string tag in enemyTags)
        {
            if (GameObject.FindGameObjectWithTag(tag) != null) { hasEnemy = true; break; }
        }
        if (!hasEnemy) return;
        if (playerTransform == null || bulletPrefab == null) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            float dir = Mathf.Sign(playerTransform.localScale.x);
            Vector3 offset = bowOffset;
            offset.x *= dir;
            Vector3 bowTargetPos = playerTransform.position + offset;

            // (1) 원래 활 비활성화
            arrowIsFlying = false;
            if (bowInstance != null && isBowActive)
            {
                bowInstance.transform.DOKill();
                bowInstance.transform.position = bowTargetPos;
                bowInstance.SetActive(false);
                isBowActive = false;
            }
            Vector3 firePos = bowTargetPos;

            // (2) 이펙트용 활 따라다니기 연출 켜기
            if (effectBowInstance != null)
            {
                effectBowInstance.SetActive(true);
                effectBowInstance.transform.position = firePos;
                effectBowInstance.transform.rotation = bowInstance != null ? bowInstance.transform.rotation : Quaternion.identity;
                effectBowInstance.transform.localScale = bowInstance != null ? bowInstance.transform.localScale : Vector3.one;
                effectBowInstance.transform.DOKill();
                effectBowInstance.transform.DOScale(effectBowInstance.transform.localScale * 1.2f, 0.1f)
                    .SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutBack);
            }

            // (3) 화살 준비 (이펙트 활/화살 모두 플레이어 따라다니는 모드로)
            GameObject bullet = GameManager.Instance.poolManager.SpawnFromPool(
                bulletPrefab.name, firePos, Quaternion.identity);
            lastArrowAI = bullet.GetComponent<BulletAI>();
            if (lastArrowAI != null)
            {
                lastArrowAI.SetSpawnOffset(offset);     // Offset설정
            }

            timer = 0f;
            // 일정 시간 뒤 (활·화살 모두 추적모드 진입) 
            StartCoroutine(ReleaseArrowAfterDelay(0.4f));
        }

        // 쏘기 전까진 effectBowInstance, 화살 전부 계속 따라다님
        SyncBowAndArrowToPlayer();

        // 각도도 맞춰주기(가장 가까운 적 방향)
        SyncBowAndArrowDirection();
    }

    void UpdateBowFollowPlayer()
    {
        if (playerTransform == null || bowInstance == null) return;
        Vector3 offset = bowOffset;
        float dir = Mathf.Sign(playerTransform.localScale.x);
        offset.x *= dir;

        Vector3 targetPos = playerTransform.position + offset;
        bowInstance.transform.DOKill();
        bowInstance.transform.DOMove(targetPos, 0.15f).SetEase(Ease.OutQuad);

        Vector3 bowScale = bowInstance.transform.localScale;
        bowScale.x = Mathf.Abs(bowScale.x) * dir;
        bowInstance.transform.localScale = bowScale;
    }

    IEnumerator ReleaseArrowAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // 실질적으로 활만 비활성화, 화살은 코루틴 내에서 날아감
        arrowIsFlying = true;

        if (effectBowInstance != null)
        {
            effectBowInstance.SetActive(false); // 활만 사라짐(날아가지 않음)
        }

        if (bowInstance != null)
            bowInstance.SetActive(true);
        isBowActive = true;
    }

    void SyncBowAndArrowToPlayer()
    {
        if (!arrowIsFlying && playerTransform != null)
        {
            float dir = Mathf.Sign(playerTransform.localScale.x);
            Vector3 offset = bowOffset; offset.x *= dir;
            Vector3 targetPos = playerTransform.position + offset;

            if (effectBowInstance != null && effectBowInstance.activeSelf)
                effectBowInstance.transform.position = targetPos;

            if (lastArrowAI != null && lastArrowAI.isActiveAndEnabled)
                lastArrowAI.transform.position = targetPos;
        }
    }

    void SyncBowAndArrowDirection()
    {
        if (!arrowIsFlying && effectBowInstance != null && effectBowInstance.activeSelf && lastArrowAI != null)
        {
            float angle = ComputeSyncAngle(effectBowInstance.transform.position);
            effectBowInstance.transform.rotation = Quaternion.Euler(0, 0, angle - 180f); // 활만 180도 돌림
            lastArrowAI.SyncSetRotation(angle);  // 화살은 즉시 각도 적용
        }
    }

    float ComputeSyncAngle(Vector3 fromPos)
    {
        Transform closest = FindClosestEnemy(fromPos);
        if (closest != null)
        {
            Vector3 dir = (closest.position - fromPos).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            return angle;
        }
        return 0f;
    }
    Transform FindClosestEnemy(Vector3 fromPos)
    {
        string[] enemyTags = { "Enemy", "DashEnemy", "LongRangeEnemy", "PotionEnemy" };
        float closestDist = Mathf.Infinity;
        Transform closest = null;
        foreach (string tag in enemyTags)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject enemy in enemies)
            {
                float dist = Vector3.Distance(fromPos, enemy.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = enemy.transform;
                }
            }
        }
        return closest;
    }
}
