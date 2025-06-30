using UnityEngine;

public class FallingLightningEffect : MonoBehaviour
{
    public Vector3 targetPosition;
    public float fallSpeed = 30f;
    public GameObject impactEffectPrefab;  // 바닥 충돌 이펙트 (예: 스파크 파티클)
    public float impactEffectDuration = 1f;  // 폭발 이펙트 유지 시간 (초)

    private bool isFalling = true;
    private Vector3 fallDirection;

    void Start()
    {
        // 도착 지점을 y축으로 살짝 위로 보정 (예: 1.5f 위로)
        targetPosition += new Vector3(0f, 1.5f, 0f);

        // 시작 위치를 목표 위치에서 위로 5f 만큼 띄우기
        transform.position = targetPosition + Vector3.up * 5f;

        // 낙하 방향 계산
        fallDirection = (targetPosition - transform.position).normalized;

        // 방향에 맞게 Z축 회전
        float angle = Mathf.Atan2(fallDirection.y, fallDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Update()
    {
        if (!isFalling) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, fallSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) <= 0.05f)
        {
            isFalling = false;
            OnImpact();
        }
    }

    void OnImpact()
    {
        if (impactEffectPrefab != null)
        {
            GameObject effect = Instantiate(impactEffectPrefab, targetPosition, Quaternion.identity);
            Destroy(effect, impactEffectDuration);  // 이펙트 일정 시간 후 삭제
        }

        Destroy(gameObject);
    }
}
