using UnityEngine;

public class PlayerEatHeart : MonoBehaviour
{

    private void Start()
    {
        //transform.localScale = GameManager.Instance.playerStats.size;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameManager.Instance.playerStats = GameManager.Instance.playerStats;
        if (other.CompareTag("Heart"))
        {
            //    // stats �� ���� ����
            //    GameManager.Instance.playerStats.size += new Vector3(1f, 1f, 1f);

            //    // �÷��̾� �����Ͽ� ����
            //    transform.localScale = GameManager.Instance.playerStats.size;


            // Heart ����
            Destroy(other.gameObject);
        }
    }
}
