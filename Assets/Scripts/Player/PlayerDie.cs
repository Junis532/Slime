using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerDie : MonoBehaviour
{
    private bool isDead = false;

    [Header("���� �г� ����")]
    public RectTransform deathPanel;            // ���� UI �г�
    public CanvasGroup deathCanvasGroup;        // ���� UI ���̵��

    void Update()
    {
        if (!isDead && GameManager.Instance.playerStats.currentHP <= 0)
        {
            isDead = true;
            PlayDeathSequence();
        }
    }

    public void PlayDeathSequence()
    {
        Sequence deathSequence = DOTween.Sequence();

        Vector3 backwardDir = -transform.right * 0.5f;

        deathSequence.Append(transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack));
        deathSequence.Join(transform.DOMove(transform.position + backwardDir, 0.5f).SetEase(Ease.OutQuad));
        deathSequence.Join(transform.DORotate(new Vector3(0, 0, 360 * 3), 0.5f, RotateMode.FastBeyond360));

        deathSequence.OnComplete(() =>
        {
            Destroy(gameObject);

            // ���� UI ���� �ִϸ��̼�
            if (deathCanvasGroup != null && deathPanel != null)
            {
                deathCanvasGroup.alpha = 0f;

                // �г� �̵�
                deathPanel.DOAnchorPosY(0f, 0.7f).SetEase(Ease.OutCubic)
                    .OnComplete(() =>
                    {
                        // ��ġ �ִϸ��̼� ���� �� ���� ����
                        GameManager.Instance.ChangeStateToEnd();
                    });

                // ���̵� ��
                deathCanvasGroup.DOFade(1f, 0.7f);
            }
            else
            {
                // UI�� ������� �ʾ����� �ٷ� ���� ��ȯ
                GameManager.Instance.ChangeStateToEnd();
            }
        });
    }
}
