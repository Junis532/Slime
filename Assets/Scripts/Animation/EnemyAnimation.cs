using UnityEngine;
using System.Collections.Generic;

public class EnemyAnimation : MonoBehaviour
{
    [System.Serializable]
    public enum State { Idle, Move };

    public List<Sprite> idleSprites;
    public List<Sprite> moveSprites;

    public float frameRate = 0.1f;

    private SpriteRenderer spriteRenderer;
    private float timer;                   // �ִϸ��̼� ������ ��ȯ�� ���� Ÿ�̸�
    private int currentFrame;              // ���� ǥ�� ���� ������ �ε���
    public State currentState;            // ���� �÷��̾� ����
    private List<Sprite> currentSprites;   // ���� ���¿� �ش��ϴ� ��������Ʈ ����Ʈ

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // ��������Ʈ ����Ʈ�� ��������� �������� ����
        if (currentSprites == null || currentSprites.Count == 0) return;

        // �ð� ��� ����
        timer += Time.deltaTime;

        // ������ ������ ������ �Ѱ�ٸ� ���� ���������� ��ȯ
        if (timer >= frameRate)
        {
            timer = 0f;
            currentFrame = (currentFrame + 1) % currentSprites.Count;
            spriteRenderer.sprite = currentSprites[currentFrame];
        }
    }

    // ���¸� �޾Ƽ� �ش� ������ �ִϸ��̼��� ���
    public void PlayAnimation(State newState)
    {
        // ���� ���·� �ٽ� ��ȯ�Ϸ��� �ϸ� ����
        if (newState == currentState) return;

        // ���� ���� �� �ʱ�ȭ
        currentState = newState;
        currentFrame = 0;
        timer = 0f;

        // ���¿� �´� ��������Ʈ ����Ʈ�� ����
        currentSprites = (currentState == State.Idle) ? idleSprites : moveSprites;

        // ù ��° ��������Ʈ�� �ʱ�ȭ
        spriteRenderer.sprite = currentSprites[0];
    }
}