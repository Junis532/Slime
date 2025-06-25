using UnityEngine;

public class JoystickDirectionIndicator4 : MonoBehaviour
{
    [Header("조이스틱 (선택 사항)")]
    public VariableJoystick joystick; // ← 여기에 조이스틱 연결하세요

    [Header("스킬 범위 스프라이트")]
    public GameObject directionSpritePrefab;

    [Header("스킬 범위 중앙값 설정")]
    public float distanceFromPlayer = 0.0f;
    public float spriteBackOffset = 0.0f;

    private GameObject indicatorInstance;
    private PlayerController playerController;

    private bool isTouchingJoystick = false;
    private bool wasTouchingJoystickLastFrame = false;

    void Start()
    {
        playerController = GetComponent<PlayerController>();

        if (directionSpritePrefab != null)
        {
            indicatorInstance = Instantiate(directionSpritePrefab, transform.position, Quaternion.identity);
            indicatorInstance.SetActive(false);
        }
    }

    void Update()
    {
        Vector2 input;

        // ✅ 조이스틱이 연결되어 있다면 조이스틱 입력을 사용
        if (joystick != null)
            input = new Vector2(joystick.Horizontal, joystick.Vertical);
        else
            input = playerController.InputVector; // 아니면 Input System 입력 사용

        isTouchingJoystick = input.magnitude > 0.2f;

        if (isTouchingJoystick)
        {
            if (!indicatorInstance.activeSelf)
                indicatorInstance.SetActive(true);

            Vector3 direction = new Vector3(input.x, input.y, 0f).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            indicatorInstance.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            Vector3 basePos = transform.position + direction * distanceFromPlayer;
            Vector3 offset = -indicatorInstance.transform.up * spriteBackOffset;

            indicatorInstance.transform.position = basePos + offset;
        }
        else
        {
            if (indicatorInstance.activeSelf)
                indicatorInstance.SetActive(false);
        }

        if (!isTouchingJoystick && wasTouchingJoystickLastFrame)
        {
            OnJoystickReleased();
        }

        wasTouchingJoystickLastFrame = isTouchingJoystick;
    }

    private void OnJoystickReleased()
    {
        Debug.Log("스킬 발사!!");
        if (indicatorInstance != null)
            indicatorInstance.SetActive(false);
    }
}
