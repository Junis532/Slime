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

        spriteRenderer.DOKill();  // 혹시 이전 애니메이션 있으면 정리
        spriteRenderer.color = Color.red;  // 빨간색으로 변경
        spriteRenderer.DOColor(originalColor, 0.5f);  // 0.5초 동안 원래 색으로 돌아감

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
