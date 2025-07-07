using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SkillType
{
    None = 0,      // 스킬 없음 (기본값)
    Fireball = 1,
    Teleport = 2,
    Lightning = 3,
    Windwall = 4
}

public class JoystickDirectionIndicator3 : MonoBehaviour
{
    [Header("조이스틱 (선택 사항)")]
    public VariableJoystick joystick;

    [Header("조이스틱 투명도 조절용")]
    public CanvasGroup joystickCanvasGroup;

    [Header("조이스틱 조작 중 숨길 이미지")]
    public GameObject imageToHideWhenTouching;

    [Header("스킬 범위 스프라이트 (1~4)")]
    public List<GameObject> directionSpritePrefabs;

    [Header("스킬 범위 중앙값 설정 (1~4)")]
    public List<float> distancesFromPlayer;
    public List<float> spriteBackOffsets;

    [Header("스킬별 범위 회전 오프셋 (각도, 1~4)")]
    public List<float> skillAngleOffsets;

    [Header("파이어볼 관련")]
    public GameObject fireballPrefab;
    public Transform firePoint;

    [Header("번개 관련")]
    public GameObject lightningPrefab;
    public GameObject LightningEffectPrefab;

    [Header("바람막 관련")]
    public GameObject windWallPrefab;

    [Header("순간이동 이펙트")]
    public GameObject teleportEffectPrefab;
    public float teleportEffectDuration = 1f;

    [Header("주사위 Image (알파 조절용)")]
    public Image diceImage;

    [Header("입력 차단 캔버스")]
    public GameObject blockInputCanvas;

    private GameObject indicatorInstance;
    private int currentIndicatorIndex = -1;

    private PlayerController playerController;

    private bool isTouchingJoystick = false;
    private bool wasTouchingJoystickLastFrame = false;
    private Vector2 lastInputDirection = Vector2.right;
    private float lastInputMagnitude = 0f;

    private bool hasUsedSkill = false;
    private bool prevIsRolling = false;

    private bool isTeleportMode = false;
    private Vector3 teleportTargetPosition;

    private bool isLightningMode = false;
    private Vector3 lightningTargetPosition;

    private Vector2 lightningCastDirection;

    // 이전 blockInputCanvas 활성 상태 저장용
    private bool prevBlockInputActive = false;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        if (joystickCanvasGroup != null)
            joystickCanvasGroup.alpha = 0f;
    }

    void Update()
    {
        bool isBlockActive = blockInputCanvas != null && blockInputCanvas.activeSelf;

        if (prevBlockInputActive && !isBlockActive)
        {
            ResetInputStates();
            Debug.Log("상점 → 게임 복귀: 입력 상태 초기화 및 조이스틱 리셋 완료");
        }

        prevBlockInputActive = isBlockActive;

        if (isBlockActive)
        {
            DisableInputAndIndicators();
            return;
        }

        Vector2 input = (joystick != null) ? new Vector2(joystick.Horizontal, joystick.Vertical) : playerController.InputVector;
        isTouchingJoystick = input.magnitude > 0.2f;

        SetHideImageState(!isTouchingJoystick);

        if (DiceAnimation.currentDiceResult <= 0)
        {
            DisableInputAndIndicators();
            return;
        }
        else
        {
            SetDiceImageAlpha(1f);
        }

        if (prevIsRolling && !DiceAnimation.isRolling)
        {
            hasUsedSkill = false;
            isTeleportMode = false;
            isLightningMode = false;
        }
        prevIsRolling = DiceAnimation.isRolling;

        int currentDiceResult = DiceAnimation.currentDiceResult;

        if (hasUsedSkill)
        {
            DisableInputAndIndicators();
            return;
        }

        if (joystickCanvasGroup != null)
            joystickCanvasGroup.alpha = isTouchingJoystick ? 1f : 0f;

        if (isTouchingJoystick)
        {
            lastInputDirection = input.normalized;
            lastInputMagnitude = input.magnitude;

            SkillType currentSkill = GetMappedSkillType(currentDiceResult);

            if (currentSkill == SkillType.Teleport && isTeleportMode)
                UpdateTeleportIndicator(input);
            else if (currentSkill == SkillType.Lightning && isLightningMode)
                UpdateLightningIndicator(input);
            else
            {
                OnSkillButtonPressed();
                UpdateSkillIndicator(input, (int)currentSkill);
            }
        }
        else
        {
            if (wasTouchingJoystickLastFrame && !hasUsedSkill && lastInputMagnitude > 0.3f)
            {
                OnSkillButtonReleased();
                hasUsedSkill = true;
            }

            if (indicatorInstance != null)
                indicatorInstance.SetActive(false);

            currentIndicatorIndex = -1;
        }

        wasTouchingJoystickLastFrame = isTouchingJoystick;
        if (joystick != null)
            joystick.enabled = !DiceAnimation.isRolling && !hasUsedSkill;
    }

    void ResetInputStates()
    {
        isTouchingJoystick = false;
        wasTouchingJoystickLastFrame = false;
        lastInputDirection = Vector2.right;
        lastInputMagnitude = 0f;
        hasUsedSkill = false;
        isTeleportMode = false;
        isLightningMode = false;
        currentIndicatorIndex = -1;

        if (indicatorInstance != null)
        {
            Destroy(indicatorInstance);
            indicatorInstance = null;
        }

        if (joystickCanvasGroup != null)
            joystickCanvasGroup.alpha = 0f;

        if (joystick != null)
        {
            joystick.ResetInput();
            joystick.enabled = true;
        }
    }

    void DisableInputAndIndicators()
    {
        if (joystick != null)
            joystick.enabled = false;

        SetHideImageState(true);

        if (indicatorInstance != null)
            indicatorInstance.SetActive(false);

        currentIndicatorIndex = -1;


        isTeleportMode = false;
        isLightningMode = false;

        if (joystickCanvasGroup != null)
            joystickCanvasGroup.alpha = 0f;
    }

    void SetHideImageState(bool isVisible)
    {
        if (imageToHideWhenTouching != null)
            imageToHideWhenTouching.SetActive(isVisible);
    }

    void SetDiceImageAlpha(float alpha)
    {
        if (diceImage != null)
        {
            Color c = diceImage.color;
            c.a = alpha;
            diceImage.color = c;
        }
    }

    SkillType GetMappedSkillType(int diceResult)
    {
        // SkillSelect.FinalSkillOrder는 SkillSelect.cs에서 public static List<int> FinalSkillOrder 형태로 선언되어 있어야 합니다.
        if (SkillSelect.FinalSkillOrder == null || SkillSelect.FinalSkillOrder.Count < diceResult || diceResult <= 0)
            return SkillType.None;

        int mappedSkillNumber = SkillSelect.FinalSkillOrder[diceResult - 1];

        return (SkillType)mappedSkillNumber;
    }

    void UpdateSkillIndicator(Vector2 input, int currentSkillNumber)
    {
        int index = currentSkillNumber - 1;

        if (index >= 0 && index < directionSpritePrefabs.Count)
        {
            if (currentIndicatorIndex != index)
            {
                if (indicatorInstance != null) Destroy(indicatorInstance);
                indicatorInstance = Instantiate(directionSpritePrefabs[index], transform.position, Quaternion.identity);
                currentIndicatorIndex = index;
            }

            if (!indicatorInstance.activeSelf)
                indicatorInstance.SetActive(true);

            Vector3 direction = new Vector3(input.x, input.y, 0f).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float offsetAngle = (skillAngleOffsets != null && skillAngleOffsets.Count > index) ? skillAngleOffsets[index] : 0f;
            indicatorInstance.transform.rotation = Quaternion.Euler(0f, 0f, angle + offsetAngle);

            float dist = (distancesFromPlayer != null && distancesFromPlayer.Count > index) ? distancesFromPlayer[index] : 0f;
            float backOffset = (spriteBackOffsets != null && spriteBackOffsets.Count > index) ? spriteBackOffsets[index] : 0f;

            Vector3 basePos = transform.position + direction * dist;
            Vector3 offset = -indicatorInstance.transform.up * backOffset;
            indicatorInstance.transform.position = basePos + offset;
        }
        else
        {
            if (indicatorInstance != null)
                indicatorInstance.SetActive(false);
            currentIndicatorIndex = -1;
        }
    }

    void UpdateTeleportIndicator(Vector2 input)
    {
        int index = (int)SkillType.Teleport - 1;

        float maxDist = distancesFromPlayer.Count > index ? distancesFromPlayer[index] : 5f;
        float inputMagnitudeClamped = Mathf.Clamp01(input.magnitude);
        Vector3 direction = new Vector3(input.x, input.y, 0f).normalized;
        Vector3 basePos = transform.position + direction * maxDist * inputMagnitudeClamped;

        teleportTargetPosition = basePos;

        if (indicatorInstance != null)
        {
            indicatorInstance.SetActive(true);
            float backOffset = spriteBackOffsets.Count > index ? spriteBackOffsets[index] : 0f;
            indicatorInstance.transform.position = basePos - indicatorInstance.transform.up * backOffset;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float offsetAngle = skillAngleOffsets.Count > index ? skillAngleOffsets[index] : 0f;
            indicatorInstance.transform.rotation = Quaternion.Euler(0f, 0f, angle + offsetAngle);
        }
    }

    void UpdateLightningIndicator(Vector2 input)
    {
        int index = (int)SkillType.Lightning - 1;

        float maxDist = distancesFromPlayer.Count > index ? distancesFromPlayer[index] : 5f;
        float inputMagnitudeClamped = Mathf.Clamp01(input.magnitude);
        Vector3 direction = new Vector3(input.x, input.y, 0f).normalized;
        Vector3 basePos = transform.position + direction * maxDist * inputMagnitudeClamped;

        lightningTargetPosition = basePos;

        if (indicatorInstance != null)
        {
            indicatorInstance.SetActive(true);
            float backOffset = spriteBackOffsets.Count > index ? spriteBackOffsets[index] : 0f;
            indicatorInstance.transform.position = basePos - indicatorInstance.transform.up * backOffset;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float offsetAngle = skillAngleOffsets.Count > index ? skillAngleOffsets[index] : 0f;
            indicatorInstance.transform.rotation = Quaternion.Euler(0f, 0f, angle + offsetAngle);
        }
    }

    public void OnSkillButtonPressed()
    {
        if (joystickCanvasGroup != null)
            joystickCanvasGroup.alpha = 1f;

        int currentDiceResult = DiceAnimation.currentDiceResult;

        if (indicatorInstance != null)
        {
            Destroy(indicatorInstance);
            indicatorInstance = null;
            currentIndicatorIndex = -1;
        }

        SkillType currentSkill = GetMappedSkillType(currentDiceResult);

        int prefabIndex = (int)currentSkill - 1;
        if (prefabIndex >= 0 && prefabIndex < directionSpritePrefabs.Count)
            SetupIndicator(prefabIndex);

        isTeleportMode = (currentSkill == SkillType.Teleport);
        isLightningMode = (currentSkill == SkillType.Lightning);
    }

    void SetupIndicator(int prefabIndex)
    {
        if (currentIndicatorIndex != prefabIndex)
        {
            if (indicatorInstance != null) Destroy(indicatorInstance);

            if (directionSpritePrefabs.Count > prefabIndex)
            {
                indicatorInstance = Instantiate(directionSpritePrefabs[prefabIndex], transform.position, Quaternion.identity);
                currentIndicatorIndex = prefabIndex;
            }
        }
    }

    public void OnSkillButtonReleased()
    {
        if (hasUsedSkill) return;

        if (joystickCanvasGroup != null)
            joystickCanvasGroup.alpha = 0f;

        SkillType currentSkill = GetMappedSkillType(DiceAnimation.currentDiceResult);

        switch (currentSkill)
        {
            case SkillType.Fireball:
                ShootFireball();
                break;
            case SkillType.Teleport:
                if (isTeleportMode)
                {
                    TeleportPlayer(teleportTargetPosition);
                    isTeleportMode = false;
                }
                break;
            case SkillType.Lightning:
                if (isLightningMode)
                {
                    lightningCastDirection = lastInputDirection;
                    CastLightning(lightningTargetPosition);
                    isLightningMode = false;
                }
                break;
            case SkillType.Windwall:
                SpawnWindWall();
                break;
            default:
                Debug.Log("해당 스킬은 아직 구현되지 않았습니다.");
                break;
        }

        if (diceImage != null)
        {
            StartCoroutine(BlinkDiceImage());
        }

        Debug.Log("스킬 발사!!");

        hasUsedSkill = true;
    }

    private IEnumerator BlinkDiceImage()
    {
        diceImage.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        diceImage.gameObject.SetActive(true);
    }

    private void ShootFireball()
    {
        if (fireballPrefab == null || firePoint == null) return;

        GameObject fireballObj = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        Vector2 shootDir = lastInputDirection;
        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
        fireballObj.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        FireballProjectile fireball = fireballObj.GetComponent<FireballProjectile>();
        if (fireball != null) fireball.Init(shootDir);
    }

    private void TeleportPlayer(Vector3 targetPos)
    {
        if (teleportEffectPrefab != null)
        {
            GameObject effect = Instantiate(teleportEffectPrefab, targetPos, Quaternion.identity);
            Destroy(effect, teleportEffectDuration);
        }

        transform.position = targetPos;
    }

    private void CastLightning(Vector3 targetPos)
    {
        StartCoroutine(LightningStrikeSequence(targetPos));
    }

    private IEnumerator LightningStrikeSequence(Vector3 targetPos)
    {
        int strikeCount = 3;
        float strikeOnDuration = 0.2f;
        float strikeOffDuration = 0.23f;

        Vector2 direction = lightningCastDirection.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject lightning = null;
        LightningDamage ld = null;

        for (int i = 0; i < strikeCount; i++)
        {
            float fallDelay = 0f;

            if (LightningEffectPrefab != null)
            {
                Vector3 startPos = targetPos + Vector3.up * 5f;
                GameObject fallingEffect = Instantiate(LightningEffectPrefab, startPos, Quaternion.identity);

                FallingLightningEffect fallScript = fallingEffect.GetComponent<FallingLightningEffect>();
                if (fallScript != null)
                {
                    fallScript.targetPosition = targetPos;

                    float distance = Vector3.Distance(startPos, targetPos);
                    fallDelay = distance / fallScript.fallSpeed;
                }
            }

            yield return new WaitForSeconds(fallDelay);

            if (lightning == null && lightningPrefab != null)
            {
                lightning = Instantiate(lightningPrefab, targetPos, Quaternion.Euler(0f, 0f, angle));
                ld = lightning.GetComponent<LightningDamage>();
            }
            else if (lightning != null)
            {
                lightning.transform.position = targetPos;
                lightning.transform.rotation = Quaternion.Euler(0f, 0f, angle);
                lightning.SetActive(true);
            }

            if (ld != null) ld.Init();

            yield return new WaitForSeconds(strikeOnDuration);

            if (lightning != null) lightning.SetActive(false);

            yield return new WaitForSeconds(strikeOffDuration);
        }

        if (lightning != null) Destroy(lightning);
    }

    private void SpawnWindWall()
    {
        if (windWallPrefab == null) return;

        GameObject wall = Instantiate(windWallPrefab, transform.position, Quaternion.identity);

        Vector2 spawnDir = lastInputDirection;
        if (spawnDir == Vector2.zero)
            spawnDir = Vector2.right;

        float angle = Mathf.Atan2(spawnDir.y, spawnDir.x) * Mathf.Rad2Deg;
        wall.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
