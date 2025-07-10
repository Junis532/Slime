using UnityEngine;

public class PlayerWall : MonoBehaviour
{
    public Collider2D playerCollider;   // �÷��̾� �ݶ��̴� (�ν����Ϳ��� ����)
    private Collider2D wallCollider;    // �� �ݶ��̴� (�� ��ũ��Ʈ�� ���� ������Ʈ�� �ݶ��̴�)

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

            // �÷��̾� �ݶ��̴� 4�� �𼭸� ��ǥ
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
                // �÷��̾� �̵� ����
                PlayerController playerCtrl = other.GetComponent<PlayerController>();
                if (playerCtrl != null)
                {
                    playerCtrl.canMove = false;
                }

                // ü�� 0 ó��
                GameManager.Instance.playerStats.currentHP = 0;
                Debug.Log("�÷��̾ �� ������ ������ �����ϴ�. HP 0 ó��!");

                // ��� �� ����� ����
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
        float duration = 1.0f;  // ��ҿ� �ɸ� �ð� (1��)
        float elapsed = 0f;
        Vector3 originalScale = player.transform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            player.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            yield return null;
        }

        // ������ �۾��� �� ������Ʈ ��Ȱ��ȭ
        player.SetActive(false);

        yield break;
    }
}
