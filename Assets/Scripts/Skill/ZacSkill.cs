using UnityEngine;
using DG.Tweening;

public class ZacSkill : MonoBehaviour
{
    public GameObject piecePrefab;     // ������ ������Ʈ ������
    public int pieceCount = 5;         // ƨ�ܳ��� ���� ��
    public float radius = 1.5f;        // ƨ�ܳ��� ���� ���� �ݰ�
    public float jumpPower = 1f;       // ƨ��� ����
    public float jumpDuration = 0.5f;  // ���� ���� �ð�

    public void SpawnPieces()
    {
        for (int i = 0; i < pieceCount; i++)
        {
            Vector3 spawnPos = transform.position;

            GameObject piece = Instantiate(piecePrefab, spawnPos, Quaternion.identity);

            // Collider2D ��Ȱ��ȭ
            Collider2D col = piece.GetComponent<Collider2D>();
            if (col != null)
                col.enabled = false;

            Vector2 randomDir = Random.insideUnitCircle.normalized;
            Vector3 targetPos = spawnPos + (Vector3)randomDir * radius;

            // DOTween���� ���� ����
            piece.transform.DOJump(
                targetPos,
                jumpPower,
                1,
                jumpDuration
            )
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // ���� �Ϸ� �� Collider Ȱ��ȭ
                if (col != null)
                    col.enabled = true;
            });
        }
    }
}
