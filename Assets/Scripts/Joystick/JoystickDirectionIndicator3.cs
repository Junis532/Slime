using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    void Start()
    {
        playerController = GetComponent<PlayerController>();
        if (joystickCanvasGroup != null)
            joystickCanvasGroup.alpha = 0f;
    }

    void Update()
    {
        if (DiceAnimation.currentDiceResult <= 0)
        {
            joystick.enabled = false;
            if (joystickCanvasGroup != null) joystickCanvasGroup.alpha = 0f;
            if (imageToHideWhenTouching != null) imageToHideWhenTouching.SetActive(true);
            if (indicatorInstance != null) indicatorInstance.SetActive(false);
            currentIndicatorIndex = -1;
            return;
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
            isTouchingJoystick = false;
            if (imageToHideWhenTouching != null) imageToHideWhenTouching.SetActive(true);
            if (indicatorInstance != null) indicatorInstance.SetActive(false);
            currentIndicatorIndex = -1;
            if (joystickCanvasGroup != null) joystickCanvasGroup.alpha = 0f;
            joystick.enabled = false;
            return;
        }

        Vector2 input = (joystick != null) ? new Vector2(joystick.Horizontal, joystick.Vertical) : playerController.InputVector;
        isTouchingJoystick = input.magnitude > 0.2f;

        if (joystickCanvasGroup != null)
            joystickCanvasGroup.alpha = isTouchingJoystick ? 1f : 0f;

        if (isTouchingJoystick)
        {
            lastInputDirection = input.normalized;
            lastInputMagnitude = input.magnitude;

            if (currentDiceResult == 2 && isTeleportMode)
            {
                UpdateTeleportIndicator(input);
            }
            else if (currentDiceResult == 3 && isLightningMode)
            {
                UpdateLightningIndicator(input);
            }
            else
            {
                OnSkillButtonPressed();
                UpdateSkillIndicator(input, currentDiceResult);
            }
        }
        else
        {
            if (wasTouchingJoystickLastFrame && !hasUsedSkill && lastInputMagnitude > 0.3f)
            {
                OnSkillButtonReleased();
                hasUsedSkill = true;
            }

            if (imageToHideWhenTouching != null) imageToHideWhenTouching.SetActive(true);
            if (indicatorInstance != null) indicatorInstance.SetActive(false);
            currentIndicatorIndex = -1;
        }

        wasTouchingJoystickLastFrame = isTouchingJoystick;
        joystick.enabled = !DiceAnimation.isRolling && !hasUsedSkill;
    }

    void UpdateSkillIndicator(Vector2 input, int currentDiceResult)
    {
        if (imageToHideWhenTouching != null)
            imageToHideWhenTouching.SetActive(false);

        if (currentDiceResult >= 1 && currentDiceResult <= directionSpritePrefabs.Count)
        {
            int index = currentDiceResult - 1;

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
        float maxDist = (distancesFromPlayer != null && distancesFromPlayer.Count > 1) ? distancesFromPlayer[1] : 5f;
        float inputMagnitudeClamped = Mathf.Clamp01(input.magnitude);
        Vector3 direction = new Vector3(input.x, input.y, 0f).normalized;
        Vector3 basePos = transform.position + direction * maxDist * inputMagnitudeClamped;

        teleportTargetPosition = basePos;

        if (indicatorInstance != null)
        {
            indicatorInstance.SetActive(true);
            float backOffset = (spriteBackOffsets != null && spriteBackOffsets.Count > 1) ? spriteBackOffsets[1] : 0f;
            indicatorInstance.transform.position = basePos - indicatorInstance.transform.up * backOffset;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float offsetAngle = (skillAngleOffsets != null && skillAngleOffsets.Count > 1) ? skillAngleOffsets[1] : 0f;
            indicatorInstance.transform.rotation = Quaternion.Euler(0f, 0f, angle + offsetAngle);
        }
    }

    void UpdateLightningIndicator(Vector2 input)
    {
        float maxDist = (distancesFromPlayer != null && distancesFromPlayer.Count > 2) ? distancesFromPlayer[2] : 5f;
        float inputMagnitudeClamped = Mathf.Clamp01(input.magnitude);
        Vector3 direction = new Vector3(input.x, input.y, 0f).normalized;
        Vector3 basePos = transform.position + direction * maxDist * inputMagnitudeClamped;

        lightningTargetPosition = basePos;

        if (indicatorInstance != null)
        {
            indicatorInstance.SetActive(true);
            float backOffset = (spriteBackOffsets != null && spriteBackOffsets.Count > 2) ? spriteBackOffsets[2] : 0f;
            indicatorInstance.transform.position = basePos - indicatorInstance.transform.up * backOffset;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float offsetAngle = (skillAngleOffsets != null && skillAngleOffsets.Count > 2) ? skillAngleOffsets[2] : 0f;
            indicatorInstance.transform.rotation = Quaternion.Euler(0f, 0f, angle + offsetAngle);
        }
    }

    public void OnSkillButtonPressed()
    {
        if (joystickCanvasGroup != null)
            joystickCanvasGroup.alpha = 1f;

        int currentDiceResult = DiceAnimation.currentDiceResult;

        if (currentDiceResult == 2)
        {
            isTeleportMode = true;
            SetupIndicator(1);
        }
        else if (currentDiceResult == 3)
        {
            isLightningMode = true;
            SetupIndicator(2);
        }
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
        if (joystickCanvasGroup != null)
            joystickCanvasGroup.alpha = 0f;

        int currentDiceResult = DiceAnimation.currentDiceResult;

        switch (currentDiceResult)
        {
            case 1:
                ShootFireball();
                break;
            case 2:
                if (isTeleportMode)
                {
                    TeleportPlayer(teleportTargetPosition);
                    isTeleportMode = false;
                }
                break;
            case 3:
                if (isLightningMode)
                {
                    lightningCastDirection = lastInputDirection;  // <<< 시전 방향 저장
                    CastLightning(lightningTargetPosition);
                    isLightningMode = false;
                }
                break;

            case 4:
                SpawnWindWall();
                break;
            default:
                Debug.Log("해당 스킬은 아직 구현되지 않았습니다.");
                break;
        }

        Debug.Log("스킬 발사!!");
    }

    private void ShootFireball()
    {
        if (fireballPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Fireball prefab or firePoint not assigned.");
            return;
        }

        GameObject fireballObj = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        Vector2 shootDir = lastInputDirection;
        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
        fireballObj.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        FireballProjectile fireball = fireballObj.GetComponent<FireballProjectile>();
        if (fireball != null)
        {
            fireball.Init(shootDir);
        }
    }

    private void TeleportPlayer(Vector3 targetPos)
    {
        transform.position = targetPos;
        Debug.Log($"플레이어 텔레포트: {targetPos}");
    }

    private void CastLightning(Vector3 targetPos)
    {
        StartCoroutine(LightningStrikeSequence(targetPos));
    }

    private IEnumerator LightningStrikeSequence(Vector3 targetPos)
    {
        int strikeCount = 3;
        float strikeOnDuration = 0.2f;   // 켜져 있는 시간
        float strikeOffDuration = 0.1f;  // 꺼져 있는 시간

        Vector2 direction = lightningCastDirection.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject lightning = null;
        LightningDamage ld = null;

        GameObject lightningEffect = null;

        for (int i = 0; i < strikeCount; i++)
        {
            // lightningPrefab 깜빡임
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

            if (ld != null)
                ld.Init();

            // LightningEffectPrefab 깜빡임
            if (lightningEffect == null && LightningEffectPrefab != null)
            {
                Vector3 effectPos = targetPos + Vector3.up * 2f;
                lightningEffect = Instantiate(LightningEffectPrefab, effectPos, Quaternion.identity);
            }
            else if (lightningEffect != null)
            {
                lightningEffect.transform.position = targetPos + Vector3.up * 2f;
                lightningEffect.SetActive(true);
            }

            yield return new WaitForSeconds(strikeOnDuration);

            if (lightning != null)
                lightning.SetActive(false);

            if (lightningEffect != null)
                lightningEffect.SetActive(false);

            yield return new WaitForSeconds(strikeOffDuration);
        }

        if (lightning != null)
            Destroy(lightning);

        if (lightningEffect != null)
            Destroy(lightningEffect);
    }


    private void SpawnWindWall()
    {
        if (windWallPrefab == null)
        {
            Debug.LogWarning("WindWall prefab not assigned.");
            return;
        }

        GameObject wall = Instantiate(windWallPrefab, transform.position, Quaternion.identity);

        Vector2 spawnDir = lastInputDirection;
        if (spawnDir == Vector2.zero)
            spawnDir = Vector2.right;

        float angle = Mathf.Atan2(spawnDir.y, spawnDir.x) * Mathf.Rad2Deg;
        wall.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Debug.Log($"바람막 생성 at {transform.position}, angle {angle}");
    }
}
