using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : MonoBehaviour
{
    public Slider hpSlider;
    private Transform target; // ���� ���(��)
    private Vector3 offset = new Vector3(0, 0.5f, 0); // HP�� ��ġ ������

    public void Init(Transform target, float maxHP)
    {
        this.target = target;
        hpSlider.maxValue = maxHP;
        hpSlider.value = maxHP;
    }

    public void SetHP(float hp)
    {
        hpSlider.value = hp;
    }

    void LateUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        // ���� ��ġ �� ĵ���� ��ġ ��ȯ
        Vector3 worldPos = target.position + offset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        transform.position = screenPos;
    }
}
