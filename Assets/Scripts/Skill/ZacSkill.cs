using UnityEngine;
using DG.Tweening;

public class ZacSkill : MonoBehaviour
{
    public GameObject piecePrefab;     // ������ ������Ʈ ������
    public int pieceCount = 5;         // ƨ�ܳ��� ���� ��
    public float radius = 1.5f;        // ƨ�ܳ��� ���� ���� �ݰ�
    public float jumpPower = 1f;       // ƨ��� ����
    public float jumpDuration = 0.5f;  // ���� ���� �ð�
    public float pieceLifetime = 3f;   // �� �� �� ��������

    public void SpawnPieces()
    {
        for (int i = 0; i < pieceCount; i++)
        {
            Vector3 spawnPos = transform.position;

            GameObject piece = Instantiate(piecePrefab, spawnPos, Quaternion.identity);

            Vector2 randomDir = Random.insideUnitCircle.normalized;
            Vector3 targetPos = spawnPos + (Vector3)randomDir * radius;

            // DOTween���� ���� ����
            piece.transform.DOJump(
                targetPos,           // ���� ��ġ
                jumpPower,           // ����
                1,                   // ���� Ƚ��
                jumpDuration         // ���� �ð�
            ).SetEase(Ease.OutQuad);

            // ���� �ð� �� �ı�
            Destroy(piece, pieceLifetime);
        }
    }
}
