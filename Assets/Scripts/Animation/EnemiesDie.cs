using DG.Tweening;
using UnityEngine;

public class EnemiesDie : MonoBehaviour
{
    private bool isLive = true;

    [Header("죽을 때 드랍할 코인")]
    public GameObject coinPrefab;
    public void Die()
    {
        if (!isLive) return;

        isLive = false;

        // 코인 생성
        if (coinPrefab != null)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }

        // 죽는 연출 시작 (DOTween 시퀀스)
        Sequence deathSequence = DOTween.Sequence();

        // 살짝 뒤로 밀기 (현재 바라보는 반대 방향으로 0.5만큼)
        Vector3 backwardDir = -transform.right * 0.5f;

        // 크기 줄이기 (0.5초 동안)
        deathSequence.Append(transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack));

        // 뒤로 밀리기
        deathSequence.Join(transform.DOMove(transform.position + backwardDir, 0.5f).SetEase(Ease.OutQuad));

        // 빠른 Z축 회전 (360도 × 3회전)
        deathSequence.Join(transform.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360));

        // 연출 끝나면 오브젝트 삭제
        deathSequence.OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}
