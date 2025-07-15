using UnityEngine;
using DG.Tweening;

public class PlayerDamaged : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void PlayDamageEffect()
    {
        if (spriteRenderer == null) return;

        spriteRenderer.DOKill();  // Ȥ�� ���� �ִϸ��̼� ������ ����
        spriteRenderer.color = Color.red;  // ���������� ����
        spriteRenderer.DOColor(originalColor, 0.5f);  // 0.5�� ���� ���� ������ ���ư�

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            var zacSkill = playerObj.GetComponent<ZacSkill>();
            if (zacSkill != null)
            {
                if (zacSkill.enabled)
                {
                    zacSkill.SpawnPieces();

                }
            }
        }
    }
}
