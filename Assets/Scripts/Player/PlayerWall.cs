using UnityEngine;

public class PlayerWall : MonoBehaviour
{
    public Collider2D playerCollider;
    private Collider2D wallCollider;


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
        if (other == playerCollider)
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

                GameManager.Instance.playerStats.currentHP = 0;
            }
        }
    }
}