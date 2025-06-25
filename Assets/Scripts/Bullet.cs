using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Transform target;        // 따라갈 타겟
    public float speed = 5f;
    public float rotateSpeed = 200f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject); // 타겟이 없으면 사라짐
            return;
        }

        // 방향 벡터 계산
        Vector2 direction = ((Vector2)target.position - rb.position).normalized;

        // 회전
        float rotateAmount = Vector3.Cross(direction, transform.right).z;
        rb.angularVelocity = -rotateAmount * rotateSpeed;

        // 앞으로 이동
        rb.linearVelocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == target)
        {
            // 타겟 맞으면 삭제
            Destroy(gameObject);
        }
    }
}
