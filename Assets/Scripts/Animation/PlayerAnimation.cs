using UnityEngine;
using System.Collections.Generic;

public class PlayerAnimation : MonoBehaviour
{
    [System.Serializable]
    public enum State { Idle, Move }

    public List<Sprite> idleSprites;
    public List<Sprite> moveSprites;

    public float frameRate = 0.1f;

    private SpriteRenderer spriteRenderer;
    private float timer;
    private int currentFrame;
    public State currentState;
    private List<Sprite> currentSprites;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        // 기본 상태로 초기화
        PlayAnimation(currentState);
    }

    void Update()
    {
        if (currentSprites == null || currentSprites.Count == 0) return;

        timer += Time.deltaTime;
        if (timer >= frameRate)
        {
            timer = 0f;
            currentFrame = (currentFrame + 1) % currentSprites.Count;
            spriteRenderer.sprite = currentSprites[currentFrame];
        }
    }

    public void PlayAnimation(State newState)
    {
        if (newState == currentState && currentSprites != null && currentSprites.Count > 0) return;

        currentState = newState;
        currentFrame = 0;
        timer = 0f;

        currentSprites = (currentState == State.Idle) ? idleSprites : moveSprites;

        if (spriteRenderer != null && currentSprites.Count > 0)
            spriteRenderer.sprite = currentSprites[0];
    }
}
