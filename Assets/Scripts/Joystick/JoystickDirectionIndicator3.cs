using UnityEngine;

public class JoystickDirectionIndicator3 : MonoBehaviour
{
    [Header("조이스틱 (선택 사항)")]
    public VariableJoystick joystick;

    [Header("조이스틱 투명도 조절용")]
    public CanvasGroup joystickCanvasGroup;

    [Header("조이스틱 조작 중 숨길 이미지")]
    public GameObject imageToHideWhenTouching;

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

        if (joystickCanvasGroup != null)
            joystickCanvasGroup.alpha = 0f; // 시작 시 숨김
    }

    void Update()
    {
        Vector2 input;

        if (joystick != null)
            input = new Vector2(joystick.Horizontal, joystick.Vertical);
        else
            input = playerController.InputVector;

        isTouchingJoystick = input.magnitude > 0.2f;

        if (isTouchingJoystick)
        {
            OnSkillButtonPressed();

            // 이미지 숨기기
            if (imageToHideWhenTouching != null)
                imageToHideWhenTouching.SetActive(false);

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
            OnSkillButtonReleased();

            // 이미지 다시 보이기
            if (imageToHideWhenTouching != null)
                imageToHideWhenTouching.SetActive(true);

            if (indicatorInstance.activeSelf)
                indicatorInstance.SetActive(false);
        }

        wasTouchingJoystickLastFrame = isTouchingJoystick;
    }

    public void OnSkillButtonPressed()
    {
        if (joystickCanvasGroup != null)
            joystickCanvasGroup.alpha = 1f; // 조이스틱 보이기
    }

    public void OnSkillButtonReleased()
    {
        if (joystickCanvasGroup != null)
            joystickCanvasGroup.alpha = 0f;

        //OnJoystickReleased(); // 스킬 발사 처리
    }

    private void OnJoystickReleased()
    {
        Debug.Log("스킬 발사!!");
        if (indicatorInstance != null)
            indicatorInstance.SetActive(false);
    }
}
