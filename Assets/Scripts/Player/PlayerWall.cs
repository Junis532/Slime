using UnityEngine;

public class PlayerWall : MonoBehaviour
{
    public Collider2D playerCollider;   // 플레이어 콜라이더 (인스펙터에서 연결)
    private Collider2D wallCollider;    // 벽 콜라이더 (이 스크립트가 붙은 오브젝트의 콜라이더)

    private bool isShrinking = false;

    void Start()
    {
        wallCollider = GetComponent<Collider2D>();
        if (playerCollider == null)
        {
            Debug.LogError("Player collider not assigned!");
            playerCollider = GameObject.Find("Player").GetComponent<Collider2D>();
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other == playerCollider && !isShrinking)
        {
            Bounds playerBounds = playerCollider.bounds;
            Bounds wallBounds = wallCollider.bounds;

            // 플레이어 콜라이더 4개 모서리 좌표
            Vector3[] playerCorners = new Vector3[4];
            playerCorners[0] = new Vector3(playerBounds.min.x, playerBounds.min.y);
            playerCorners[1] = new Vector3(playerBounds.min.x, playerBounds.max.y);
            playerCorners[2] = new Vector3(playerBounds.max.x, playerBounds.min.y);
            playerCorners[3] = new Vector3(playerBounds.max.x, playerBounds.max.y);

            bool allInside = true;
            foreach (var corner in playerCorners)
            {
                if (!wallBounds.Contains(corner))
                {
                    allInside = false;
                    break;
                }
            }

            if (allInside)
            {
                // 플레이어 이동 막기
                PlayerController playerCtrl = other.GetComponent<PlayerController>();
                if (playerCtrl != null)
                {
                    playerCtrl.canMove = false;
                }

                // 체력 0 처리
                GameManager.Instance.playerStats.currentHP = 0;
                Debug.Log("플레이어가 벽 안으로 완전히 들어갔습니다. HP 0 처리!");

                // 축소 및 사라짐 시작
                StartShrinkAndDisappear(other.gameObject);
                isShrinking = true;
            }
        }
    }

    private void StartShrinkAndDisappear(GameObject player)
    {
        StartCoroutine(ShrinkAndDisappearCoroutine(player));
    }

    private System.Collections.IEnumerator ShrinkAndDisappearCoroutine(GameObject player)
    {
        float duration = 1.0f;  // 축소에 걸릴 시간 (1초)
        float elapsed = 0f;
        Vector3 originalScale = player.transform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            player.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            yield return null;
        }

        // 완전히 작아진 후 오브젝트 비활성화
        player.SetActive(false);

        yield break;
    }
}
