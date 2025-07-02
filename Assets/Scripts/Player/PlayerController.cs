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
    public bool canMove = true;

    [Header("조이스틱")]
    public VariableJoystick joystick;

    private Vector2 keyboardInput;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    void Update()
    {
        

        if (GameManager.Instance.CurrentState == "Shop")
        {
            canMove = false;
        }
        else if (GameManager.Instance.CurrentState == "Game")
        {
            canMove = true;
        }

        if (!canMove) return;

        // ✅ 1) 키보드 입력
        keyboardInput = new Vector2(
                Keyboard.current.aKey.isPressed ? -1 : Keyboard.current.dKey.isPressed ? 1 : 0,
                Keyboard.current.sKey.isPressed ? -1 : Keyboard.current.wKey.isPressed ? 1 : 0
            );

        // ✅ 2) 조이스틱 입력
        Vector2 joystickInput = new Vector2(joystick.Horizontal, joystick.Vertical);

        // ✅ 3) 두 입력 합치기 (둘 다 누르면 합산됨)
        inputVec = keyboardInput + joystickInput;

        // ✅ 4) Normalize해서 대각선 과속 방지
        if (inputVec.magnitude > 1f)
            inputVec = inputVec.normalized;

        // ✅ 5) 이동
        currentDirection = Vector2.SmoothDamp(currentDirection, inputVec, ref currentVelocity, smoothTime);
        Vector2 nextVec = currentDirection * GameManager.Instance.playerStats.speed * Time.deltaTime;
        transform.Translate(nextVec);

        // ✅ 6) 캐릭터 방향 전환
        if (currentDirection.magnitude > 0.01f)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (currentDirection.x < 0 ? -1 : 1);
            transform.localScale = scale;
        }

        // ✅ 7) 애니메이션
        if (currentDirection == Vector2.zero)
            playerAnimation.PlayAnimation(PlayerAnimation.State.Idle);
        else
            playerAnimation.PlayAnimation(PlayerAnimation.State.Move);
    }

    // ✅ Unity New InputSystem OnMove 이벤트도 연결하고 싶으면 이거도 추가
    void OnMove(InputValue value)
    {
        // 필요시 여기에 추가적인 처리 가능
    }
}
