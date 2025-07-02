using DG.Tweening;
using UnityEngine;

public class EnemiesDie : MonoBehaviour
{
    private bool isLive = true;

    [Header("���� �� ����� ����")]
    public GameObject coinPrefab;
    public void Die()
    {
        if (!isLive) return;

        isLive = false;

        // ���� ����
        if (coinPrefab != null)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }

        // �״� ���� ���� (DOTween ������)
        Sequence deathSequence = DOTween.Sequence();

        // ��¦ �ڷ� �б� (���� �ٶ󺸴� �ݴ� �������� 0.5��ŭ)
        Vector3 backwardDir = -transform.right * 0.5f;

        // ũ�� ���̱� (0.5�� ����)
        deathSequence.Append(transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack));

        // �ڷ� �и���
        deathSequence.Join(transform.DOMove(transform.position + backwardDir, 0.5f).SetEase(Ease.OutQuad));

        // ���� Z�� ȸ�� (360�� �� 3ȸ��)
        deathSequence.Join(transform.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360));

        // ���� ������ ������Ʈ ����
        deathSequence.OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}
