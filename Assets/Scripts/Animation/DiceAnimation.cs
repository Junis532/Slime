using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DiceAnimation : MonoBehaviour
{
    [Header("주사위 이미지 (0~3 인덱스: 1~4 숫자에 대응)")]
    public List<Sprite> diceSprites;

    [Header("저장용 주사위 이미지 (noSkillUseCount에 대응)")]
    public List<Sprite> saveDiceSpritesByNoSkillCount;

    [Header("저장용 주사위 이미지 표시 UI")]
    public Image saveDiceImage;

    [Header("스킬 UI 이미지 슬롯 (1~4)")]
    public List<Image> skillSlotImages;

    [Header("조이스틱 관련")]
    public CanvasGroup joystickCanvasGroup;
    public VariableJoystick joystick;

    [Header("스킬 이미지 표시용 UI")]
    public Image skillImage;

    [Header("대기 시간 표시용 텍스트")]
    public TMP_Text waitTimerText;

    public float frameRate = 0.05f;
    public float rollDuration = 3f;
    public float waitInterval = 10f;

    private Image image;
    private Coroutine rollCoroutine;

    public static bool isRolling = false;
    public static int currentDiceResult = 0;

    public static bool hasUsedSkill = false; // 스킬 사용 여부 (공유용)
    public static int noSkillUseCount = 1;   // 리롤 중 스킬 미사용 카운트

    void Start()
    {
        image = GetComponent<Image>();
        // 시작 시 주사위 이미지 활성화
        if (image != null)
            image.gameObject.SetActive(true);

        RollOnceAtStart();

        UpdateSkillImage();
        UpdateSaveDiceImageByNoSkillCount();

        if (waitTimerText != null)
            waitTimerText.text = "";
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

        // 조이스틱 비활성화
        if (joystick != null)
            joystick.gameObject.SetActive(false);
        if (joystickCanvasGroup != null)
            joystickCanvasGroup.gameObject.SetActive(false);

        // 이미지 활성화
        if (image != null)
            image.gameObject.SetActive(true);

        float elapsed = 0f;
        int animFrame = 0;

        while (elapsed < rollDuration)
        {
            image.sprite = diceSprites[animFrame];
            animFrame = (animFrame + 1) % diceSprites.Count;
            elapsed += frameRate;
            yield return new WaitForSeconds(frameRate);
        }

        int result = Random.Range(1, 2); // 1~4 범위로 수정
        currentDiceResult = result;
        image.sprite = diceSprites[result - 1];
        Debug.Log($"시작 시 주사위 결과: {result}");

        UpdateSkillImage();
        UpdateSaveDiceImageByNoSkillCount();

        isRolling = false;

        // 조이스틱 활성화
        if (joystick != null)
            joystick.gameObject.SetActive(true);
        if (joystickCanvasGroup != null)
            joystickCanvasGroup.gameObject.SetActive(true);

        if (waitTimerText != null)
            waitTimerText.text = "";
    }

    void UpdateSkillImage()
    {
        if (skillImage != null && skillSlotImages != null && skillSlotImages.Count >= currentDiceResult && currentDiceResult > 0)
        {
            skillImage.sprite = skillSlotImages[currentDiceResult - 1].sprite;
            skillImage.enabled = true;
        }
    }

    void UpdateSaveDiceImageByNoSkillCount()
    {
        if (saveDiceImage != null && saveDiceSpritesByNoSkillCount != null &&
            noSkillUseCount > 0 && noSkillUseCount <= saveDiceSpritesByNoSkillCount.Count)
        {
            saveDiceImage.sprite = saveDiceSpritesByNoSkillCount[noSkillUseCount - 1];
            saveDiceImage.enabled = true;
        }
        else if (saveDiceImage != null)
        {
            saveDiceImage.enabled = false;
        }
    }

    public void StartRollingLoop()
    {
        if (rollCoroutine == null)
        {
            rollCoroutine = StartCoroutine(RollingLoopRoutine());
            Debug.Log("리롤 루프 시작");
        }
    }

    public void StopRollingLoop()
    {
        if (rollCoroutine != null)
        {
            StopCoroutine(rollCoroutine);
            rollCoroutine = null;
            Debug.Log("주사위 루프 멈춤");

            // 이미지 비활성화
            if (image != null)
                image.gameObject.SetActive(false);

            if (waitTimerText != null)
                waitTimerText.text = "";
        }
    }

    IEnumerator RollingLoopRoutine()
    {
        while (true)
        {
            Debug.Log("리롤 루프 대기 시작...");
            float waitTime = waitInterval;
            while (waitTime > 0f)
            {
                if (waitTimerText != null)
                    waitTimerText.text = $"{waitTime:F1}";

                waitTime -= Time.deltaTime;
                yield return null;
            }

            Debug.Log("리롤 루프 실행 중...");

            isRolling = true;

            // 조이스틱 비활성화
            if (joystick != null)
                joystick.gameObject.SetActive(false);
            if (joystickCanvasGroup != null)
                joystickCanvasGroup.gameObject.SetActive(false);

            // 이미지 활성화
            if (image != null)
                image.gameObject.SetActive(true);

            float elapsed = 0f;
            int animFrame = 0;

            while (elapsed < rollDuration)
            {
                image.sprite = diceSprites[animFrame];
                animFrame = (animFrame + 1) % diceSprites.Count;
                elapsed += frameRate;
                yield return new WaitForSeconds(frameRate);
            }

            int result = Random.Range(1, 2); // 1~4 범위로 수정
            currentDiceResult = result;
            image.sprite = diceSprites[result - 1];
            Debug.Log($"주사위 결과: {result}");
            Debug.Log($"{hasUsedSkill}");

            UpdateSkillImage();

            // 스킬 미사용 시 카운트 +1 (최대 4)
            if (!hasUsedSkill)
            {
                noSkillUseCount = Mathf.Min(noSkillUseCount + 1, 4);
                Debug.Log($"스킬 미사용 noSkillUseCount: {noSkillUseCount}"); // 로그 추가
            }
            else
            {
                noSkillUseCount = 1;
                Debug.Log("스킬 사용, noSkillUseCount 초기화 1");
            }

            UpdateSaveDiceImageByNoSkillCount();

            isRolling = false;

            // 조이스틱 활성화
            if (joystick != null)
                joystick.gameObject.SetActive(true);
            if (joystickCanvasGroup != null)
                joystickCanvasGroup.gameObject.SetActive(true);

            if (waitTimerText != null)
                waitTimerText.text = "";
        }
    }

    // 저장 버튼에서 호출
    public void OnSaveButtonPressed()
    {
        // 저장 시 현재 noSkillUseCount 값 유지하며 로그만 출력
        Debug.Log($"저장 버튼 누름, noSkillUseCount: {noSkillUseCount}");
    }

    // 외부에서 강제 리롤 정지
    public void ForceStopRolling()
    {
        if (rollCoroutine != null)
        {
            StopCoroutine(rollCoroutine);
            rollCoroutine = null;

            isRolling = false;

            // 조이스틱 활성화
            if (joystick != null)
                joystick.gameObject.SetActive(true);
            if (joystickCanvasGroup != null)
                joystickCanvasGroup.gameObject.SetActive(true);

            // 이미지 비활성화
            if (image != null)
                image.gameObject.SetActive(false);

            if (waitTimerText != null)
                waitTimerText.text = "";

            Debug.Log("버튼으로 리롤 강제 정지됨");
        }
    }

    // 스킬 사용 시 외부에서 호출해 주세요.
    public void OnSkillUsed()
    {
        hasUsedSkill = true;
        Debug.Log("스킬 사용 플래그 설정됨");

        // 코루틴이 멈췄다면 다시 시작
        if (rollCoroutine == null)
        {
            StartRollingLoop();
            Debug.Log("리롤 루프 재시작");
        }
    }
}
