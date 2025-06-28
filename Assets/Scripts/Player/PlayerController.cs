using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Vector2 InputVector => inputVec;

    public Vector2 inputVec;
    private Vector2 currentVelocity;
    private Vector2 currentDirection;
    private PlayerAnimation playerAnimation;
    private SpriteRenderer spriteRenderer;

    public float smoothTime = 0.1f;

    public bool canMove = true;  // 이동 가능 여부 변수 추가

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    void Update()
    {
        if (!canMove) return; // 이동 불가 시 이동 로직 실행 안 함

        Vector2 input = inputVec;

        currentDirection = Vector2.SmoothDamp(currentDirection, input, ref currentVelocity, smoothTime);
        Vector2 nextVec = currentDirection * GameManager.Instance.playerStats.speed * Time.deltaTime;
        transform.Translate(nextVec);

        if (currentDirection.magnitude > 0.01f)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (currentDirection.x < 0 ? -1 : 1);
            transform.localScale = scale;
        }

        if (currentDirection == Vector2.zero)
            playerAnimation.PlayAnimation(PlayerAnimation.State.Idle);
        else
            playerAnimation.PlayAnimation(PlayerAnimation.State.Move);
    }

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }
}
