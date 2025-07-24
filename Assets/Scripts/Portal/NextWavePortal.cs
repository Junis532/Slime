using UnityEngine;

public class NextWavePortal : MonoBehaviour
{
    private bool waveStarted = false;
    private bool playerInside = false;
    private float stayTimer = 0f;
    public float requiredStayTime = 3f;

    private void Update()
    {
        if (waveStarted || !playerInside) return;

        stayTimer += Time.deltaTime;

        if (stayTimer >= requiredStayTime)
        {
            waveStarted = true;
            GameManager.Instance.waveManager.StartNextWave();
            Debug.Log("�÷��̾ 3�ʰ� ��Ż �ȿ� �ӹ��� ���� ���̺� ����!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            stayTimer = 0f; // �ð� �ʱ�ȭ
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            stayTimer = 0f; // ������ Ÿ�̸� ����
        }
    }
}
