using UnityEngine;

public class GroupController : MonoBehaviour
{
    private int aliveCount;

    void Start()
    {
        // �ڽ� ������ aliveCount �ʱ�ȭ
        aliveCount = transform.childCount;

        foreach (Transform child in transform)
        {
            EnemiesDie dieScript = child.GetComponent<EnemiesDie>();
            if (dieScript != null)
            {
                dieScript.SetGroupController(this);
            }
        }
    }

    // ���� �ڽ��� ���� ������ ȣ���
    public void OnChildDie()
    {
        aliveCount--;
        if (aliveCount <= 0)
        {
            // �׷� ��ü ��ȯ
            PoolManager.Instance.ReturnToPool(this.gameObject);
        }
    }

    // �׷� ����� �� aliveCount�� ���ʱ�ȭ �ʿ�
    void OnEnable()
    {
        aliveCount = transform.childCount;
    }
}
