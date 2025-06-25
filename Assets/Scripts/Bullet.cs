using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Transform target;        // ���� Ÿ��
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
            Destroy(gameObject); // Ÿ���� ������ �����
            return;
        }

        // ���� ���� ���
        Vector2 direction = ((Vector2)target.position - rb.position).normalized;

        // ȸ��
        float rotateAmount = Vector3.Cross(direction, transform.right).z;
        rb.angularVelocity = -rotateAmount * rotateSpeed;

        // ������ �̵�
        rb.linearVelocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == target)
        {
            // Ÿ�� ������ ����
            Destroy(gameObject);
        }
    }
}
