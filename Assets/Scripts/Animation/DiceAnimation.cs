using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;  // TextMeshPro 네임스페이스 추가

public class DiceAnimation : MonoBehaviour
{
    [Header("주사위 이미지 (0~3 인덱스: 1~4 숫자에 대응)")]
    public List<Sprite> diceSprites;

    [Header("스킬 UI 이미지 슬롯 (1~4)")]
    public List<Image> skillSlotImages;  // 각 UI 슬롯의 Image 컴포넌트에서 스프라이트 가져옴

    [Header("조이스틱 관련")]
    public CanvasGroup joystickCanvasGroup;  // 조이스틱 알파용
    public VariableJoystick joystick;        // 조이스틱 입력 막을 때 사용

    [Header("스킬 이미지 표시용 UI")]
    public Image skillImage;  // 결과에 따라 바뀔 스킬 이미지 UI

    [Header("대기 시간 표시용 텍스트")]
    public TMP_Text waitTimerText;   // 새로 추가: 남은 대기시간 표시용 UI 텍스트

    public float frameRate = 0.05f;
    public float rollDuration = 3f;
    public float waitInterval = 10f;

    private Image image;
    private Coroutine rollCoroutine;

    public static bool isRolling = false;
    public static int currentDiceResult = 0;  // 현재 주사위 결과 저장 (1~4)

    void Start()
    {
        image = GetComponent<Image>();

        RollOnceAtStart();

        // 초기 스킬 이미지 설정
        if (skillImage != null && skillSlotImages != null && skillSlotImages.Count >= 4)
        {
            skillImage.sprite = skillSlotImages[currentDiceResult - 1].sprite;
            skillImage.enabled = true;
        }

        if (waitTimerText != null)
            waitTimerText.text = "";  // 시작 시 초기화
    }

    void OnEnable()
    {
        StartRollingLoop();
    }

    void OnDisable()
    {
        StopRollingLoop();
    }

    public void RollOnceAtStart()
    {
        StartCoroutine(RollOnceCoroutine());
    }

private IEnumerator RollOnceCoroutine()
{
    isRolling = true;

    if (joystickCanvasGroup != null)
        joystickCanvasGroup.alpha = 0.4f;

    float elapsed = 0f;
    int animFrame = 0;

    // rollDuration이 0이면 애니메이션 루프 없이 바로 결과 출력
    while (elapsed < rollDuration)
    {
        image.sprite = diceSprites[animFrame];
        animFrame = (animFrame + 1) % diceSprites.Count;
        elapsed += frameRate;
        yield return new WaitForSeconds(frameRate);
    }

    // rollDuration이 0이면 위 루프는 건너뜀

    int result = Random.Range(1, 5);
    currentDiceResult = result;
    image.sprite = diceSprites[result - 1];
    Debug.Log($"시작 시 주사위 결과: {result}");

    if (skillImage != null && skillSlotImages != null && skillSlotImages.Count >= 4)
    {
        skillImage.sprite = skillSlotImages[result - 1].sprite;
        skillImage.enabled = true;
    }

    isRolling = false;

    if (joystickCanvasGroup != null)
        joystickCanvasGroup.alpha = 1f;

    if (waitTimerText != null)
        waitTimerText.text = "";
}


    public void StartRollingLoop()
    {
        if (rollCoroutine == null)
        {
            rollCoroutine = StartCoroutine(RollingLoopRoutine());
        }
    }

    public void StopRollingLoop()
    {
        if (rollCoroutine != null)
        {
            StopCoroutine(rollCoroutine);
            rollCoroutine = null;
            Debug.Log("주사위 루프 멈춤");

            if (waitTimerText != null)
                waitTimerText.text = "";  // 루프 멈출 때 텍스트 초기화
        }
    }

    IEnumerator RollingLoopRoutine()
    {
        while (true)
        {
            // ▼ 대기시간 카운트다운 표시
            float waitTime = waitInterval;
            while (waitTime > 0f)
            {
                if (waitTimerText != null)
                    waitTimerText.text = $"{waitTime:F1}";

                waitTime -= Time.deltaTime;
                yield return null;
            }

            isRolling = true;
            if (joystickCanvasGroup != null)
                joystickCanvasGroup.alpha = 0.4f;

            float elapsed = 0f;
            int animFrame = 0;

            while (elapsed < rollDuration)
            {
                image.sprite = diceSprites[animFrame];
                animFrame = (animFrame + 1) % diceSprites.Count;
                elapsed += frameRate;
                yield return new WaitForSeconds(frameRate);
            }

            int result = Random.Range(1, 5);
            currentDiceResult = result;
            image.sprite = diceSprites[result - 1];
            Debug.Log($"주사위 결과: {result}");

            // ▼ 결과에 맞는 스킬 이미지 표시
            if (skillImage != null && skillSlotImages != null && skillSlotImages.Count >= 4)
            {
                skillImage.sprite = skillSlotImages[result - 1].sprite;
                skillImage.enabled = true;
            }

            isRolling = false;
            if (joystickCanvasGroup != null)
                joystickCanvasGroup.alpha = 1f;

            // 주사위 완료 후 텍스트 초기화
            if (waitTimerText != null)
                waitTimerText.text = "";
        }
    }
}
