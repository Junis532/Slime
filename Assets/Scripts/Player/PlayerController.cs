using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public VariableJoystick joystick; // 에디터에서 UI 조이스틱 연결
    public Vector2 InputVector => inputVec;

    public Vector2 inputVec;
    private Vector2 currentVelocity;
    private Vector2 currentDirection;
    private PlayerAnimation playerAnimation;
    private SpriteRenderer spriteRenderer;

    public float smoothTime = 0.1f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    void Update()
    {
        // VariableJoystick 값 우선 사용, 없으면 Input System에서 받은 값 사용
        Vector2 input = Vector2.zero;
        if (joystick != null && joystick.Direction.magnitude > 0.1f)
            input = joystick.Direction;
        else
            input = inputVec;  // OnMove 콜백으로 받은 값

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
