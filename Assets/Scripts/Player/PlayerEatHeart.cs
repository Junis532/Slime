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
            //    // stats 값 직접 수정
            //    GameManager.Instance.playerStats.size += new Vector3(1f, 1f, 1f);

            //    // 플레이어 스케일에 적용
            //    transform.localScale = GameManager.Instance.playerStats.size;


            // Heart 제거
            Destroy(other.gameObject);
        }
    }
}
