using UnityEngine;

public class FallingLightningEffect : MonoBehaviour
{
    public Vector3 targetPosition;
    public float fallSpeed = 30f;
    public GameObject impactEffectPrefab;  // �ٴ� �浹 ����Ʈ (��: ����ũ ��ƼŬ)
    public float impactEffectDuration = 1f;  // ���� ����Ʈ ���� �ð� (��)

    private bool isFalling = true;
    private Vector3 fallDirection;

    void Start()
    {
        // ���� ������ y������ ��¦ ���� ���� (��: 1.5f ����)
        targetPosition += new Vector3(0f, 1.5f, 0f);

        // ���� ��ġ�� ��ǥ ��ġ���� ���� 5f ��ŭ ����
        transform.position = targetPosition + Vector3.up * 5f;

        // ���� ���� ���
        fallDirection = (targetPosition - transform.position).normalized;

        // ���⿡ �°� Z�� ȸ��
        float angle = Mathf.Atan2(fallDirection.y, fallDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Update()
    {
        if (!isFalling) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, fallSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) <= 0.05f)
        {
            isFalling = false;
            OnImpact();
        }
    }

    void OnImpact()
    {
        if (impactEffectPrefab != null)
        {
            GameObject effect = Instantiate(impactEffectPrefab, targetPosition, Quaternion.identity);
            Destroy(effect, impactEffectDuration);  // ����Ʈ ���� �ð� �� ����
        }

        Destroy(gameObject);
    }
}
