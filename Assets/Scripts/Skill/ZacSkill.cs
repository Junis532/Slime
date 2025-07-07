using UnityEngine;
using DG.Tweening;

public class ZacSkill : MonoBehaviour
{
    public GameObject piecePrefab;     // 떨어질 오브젝트 프리팹
    public int pieceCount = 5;         // 튕겨나올 조각 수
    public float radius = 1.5f;        // 튕겨나올 방향 범위 반경
    public float jumpPower = 1f;       // 튕기는 높이
    public float jumpDuration = 0.5f;  // 점프 지속 시간
    public float pieceLifetime = 3f;   // 몇 초 뒤 제거할지

    public void SpawnPieces()
    {
        for (int i = 0; i < pieceCount; i++)
        {
            Vector3 spawnPos = transform.position;

            GameObject piece = Instantiate(piecePrefab, spawnPos, Quaternion.identity);

            Vector2 randomDir = Random.insideUnitCircle.normalized;
            Vector3 targetPos = spawnPos + (Vector3)randomDir * radius;

            // DOTween으로 점프 연출
            piece.transform.DOJump(
                targetPos,           // 도착 위치
                jumpPower,           // 높이
                1,                   // 점프 횟수
                jumpDuration         // 지속 시간
            ).SetEase(Ease.OutQuad);

            // 일정 시간 후 파괴
            Destroy(piece, pieceLifetime);
        }
    }
}
