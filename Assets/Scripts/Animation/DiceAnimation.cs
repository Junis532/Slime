﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DiceAnimation : MonoBehaviour
{
    [Header("사용 가능 주사위 이미지 (0~3 인덱스: 1~4 숫자에 대응)")]
    public List<Sprite> diceSprites;

    [Header("스킬 사용된 주사위 이미지 (0~3 인덱스: 1~4 숫자에 대응)")]
    public List<Sprite> usedDiceSprites;

    //[Header("저장용 주사위 이미지 (noSkillUseCount에 대응)")]
    //public List<Sprite> saveDiceSpritesByNoSkillCount;

    //[Header("저장용 주사위 이미지 표시 UI")]
    //public Image saveDiceImage;

    [Header("스킬 UI 이미지 슬롯 (1~4)")]
    public List<Image> skillSlotImages;

    [Header("조이스틱 관련")]
    public CanvasGroup joystickCanvasGroup;
    public VariableJoystick joystick;

    [Header("스킬 이미지 표시용 UI")]
    public Image skillImage;

    //[Header("스킬 저장 버튼")]
    //public Button skillSaveButton;

    [Header("대기 시간 표시용 텍스트 이미지")]
    public TMP_Text waitTimerText;
    public Image CooltimeImange;

    public float frameRate = 0.05f;
    public float rollDuration = 3f;
    public int waitInterval = 10;

    private Image image;
    private Coroutine rollCoroutine;

    public static bool isRolling = false;
    public static int currentDiceResult = 0;

    public static bool hasUsedSkill = false;
    public static int noSkillUseCount = 1;

    void Start()
    {
        image = GetComponent<Image>();

        if (image == null)
        {
            Debug.LogError("❌ Image 컴포넌트가 없습니다!");
            return;
        }

        if (diceSprites == null || diceSprites.Count == 0)
        {
            Debug.LogError("❌ diceSprites가 설정되지 않았거나 비어 있습니다!");
            return;
        }

        RollOnceAtStart();
    }


    void OnEnable() => StartRollingLoop();
    void OnDisable() => StopRollingLoop();

    public void RollOnceAtStart()
    {
        StartCoroutine(RollOnceCoroutine());
    }

    private IEnumerator RollOnceCoroutine()
    {
        isRolling = true;
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

        UpdateSkillImage();
        isRolling = false;
        if (waitTimerText != null)
            waitTimerText.text = "";
    }

    IEnumerator RollingLoopRoutine()
    {
        while (true)
        {
            float waitTime = waitInterval;
            while (waitTime > 0f)
            {
                if (waitTimerText != null)
                    waitTimerText.text = $"{Mathf.CeilToInt(waitTime)}";
                waitTime -= Time.deltaTime;

                CooltimeImange.fillAmount = waitTime / waitInterval;

                yield return null;
            }

            isRolling = true;
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

            UpdateSkillImage();

            if (!hasUsedSkill)
            {
                noSkillUseCount = Mathf.Min(noSkillUseCount + 1, 4);
            }

            //UpdateSaveDiceImageByNoSkillCount();
            isRolling = false;

            if (waitTimerText != null)
                waitTimerText.text = "";
        }
    }

    void UpdateSkillImage()
    {
        if (skillImage != null && skillSlotImages.Count >= currentDiceResult && currentDiceResult > 0)
        {
            skillImage.sprite = skillSlotImages[currentDiceResult - 1].sprite;
            skillImage.enabled = true;
        }
    }

    //void UpdateSaveDiceImageByNoSkillCount()
    //{
    //    if (saveDiceImage != null && noSkillUseCount > 0 && noSkillUseCount <= saveDiceSpritesByNoSkillCount.Count)
    //    {
    //        saveDiceImage.sprite = saveDiceSpritesByNoSkillCount[noSkillUseCount - 1];
    //        saveDiceImage.enabled = true;
    //    }
    //    else if (saveDiceImage != null)
    //    {
    //        saveDiceImage.enabled = false;
    //    }
    //}

    public void StartRollingLoop()
    {
        RollOnceAtStart();

        if (rollCoroutine == null)
        {
            rollCoroutine = StartCoroutine(RollingLoopRoutine());

            if (currentDiceResult > 0 && currentDiceResult <= diceSprites.Count)
            {
                image.sprite = diceSprites[currentDiceResult - 1];
            }
        }
    }

    public void StopRollingLoop()
    {
        if (rollCoroutine != null)
        {
            StopCoroutine(rollCoroutine);
            rollCoroutine = null;

            if (waitTimerText != null)
                waitTimerText.text = "";
        }
    }

    

    // 외부에서 "세이브 주사위 또는 일반 주사위"에 따라 효과만 발생시킴 (스킬과는 무관)
    public void ExecuteSkillEffect(int effectDiceValue)
    {
        switch (effectDiceValue)
        {
            case 1:
                Debug.Log("⭐ 효과: 약한 이펙트 발동");
                break;
            case 2:
                Debug.Log("⭐ 효과: 중간 강도의 이펙트 발동");
                break;
            case 3:
                Debug.Log("⭐ 효과: 강한 이펙트 발동");
                break;
            case 4:
                Debug.Log("⭐ 효과: 궁극기급 이펙트 발동");
                break;
            default:
                Debug.LogWarning("잘못된 효과 눈금값 (1~4)");
                break;
        }
    }


    // 강제 리롤 정지
    public void ForceStopRolling()
    {
        hasUsedSkill = true;

        //if (skillSaveButton != null)
        //    skillSaveButton.gameObject.SetActive(false);

        if (rollCoroutine != null)
        {
            StopCoroutine(rollCoroutine);
            rollCoroutine = null;
            isRolling = false;

            if (waitTimerText != null)
                waitTimerText.text = "";
        }
    }

    public void OnSkillUsed()
    {
        hasUsedSkill = false;

        // 현재 눈금에 해당하는 사용된 주사위 이미지로 변경
        if (currentDiceResult > 0 && currentDiceResult <= usedDiceSprites.Count)
        {
            image.sprite = usedDiceSprites[currentDiceResult - 1];
        }

        // 기존 코루틴 정지 및 재시작
        if (rollCoroutine != null)
        {
            StopCoroutine(rollCoroutine);
            rollCoroutine = null;
        }

        rollCoroutine = StartCoroutine(RollingLoopRoutine()); // 쿨타임 즉시 재시작
    }


}
