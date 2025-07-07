using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;  // TextMeshPro ���ӽ����̽� �߰�

public class DiceAnimation : MonoBehaviour
{
    [Header("�ֻ��� �̹��� (0~3 �ε���: 1~4 ���ڿ� ����)")]
    public List<Sprite> diceSprites;

    [Header("��ų �̹��� (1~4�� ��ų)")]
    public List<Sprite> skillSprites;  // 1~4�� ��ų �̹�����

    [Header("���̽�ƽ ����")]
    public CanvasGroup joystickCanvasGroup;  // ���̽�ƽ ���Ŀ�
    public VariableJoystick joystick;        // ���̽�ƽ �Է� ���� �� ���

    [Header("��ų �̹��� ǥ�ÿ� UI")]
    public Image skillImage;  // ����� ���� �ٲ� ��ų �̹��� UI

    [Header("��� �ð� ǥ�ÿ� �ؽ�Ʈ")]
    public TMP_Text waitTimerText;   // ���� �߰�: ���� ���ð� ǥ�ÿ� UI �ؽ�Ʈ

    public float frameRate = 0.05f;
    public float rollDuration = 3f;
    public float waitInterval = 10f;

    private Image image;
    private Coroutine rollCoroutine;

    public static bool isRolling = false;
    public static int currentDiceResult = 0;  // ���� �ֻ��� ��� ���� (1~4)

    void Start()
    {
        image = GetComponent<Image>();
        currentDiceResult = 1;
        if (skillImage != null && skillSprites != null && skillSprites.Count >= 4)
        {
            skillImage.sprite = skillSprites[currentDiceResult - 1];
            skillImage.enabled = true;
        }

        if (waitTimerText != null)
            waitTimerText.text = "";  // ���� �� �ʱ�ȭ
    }

    void OnEnable()
    {
        StartRollingLoop();

    }

    void OnDisable()
    {
        StopRollingLoop();
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
            Debug.Log("�ֻ��� ���� ����");

            if (waitTimerText != null)
                waitTimerText.text = "";  // ���� ���� �� �ؽ�Ʈ �ʱ�ȭ
        }
    }

    IEnumerator RollingLoopRoutine()
    {
        while (true)
        {
            // �� ���ð� ī��Ʈ�ٿ� ǥ��
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
            Debug.Log($"�ֻ��� ���: {result}");

            // �� ����� �´� ��ų �̹��� ǥ��
            if (skillImage != null && skillSprites != null && skillSprites.Count >= 4)
            {
                skillImage.sprite = skillSprites[result - 1];
                skillImage.enabled = true;
            }

            isRolling = false;
            if (joystickCanvasGroup != null)
                joystickCanvasGroup.alpha = 1f;

            // �ֻ��� �Ϸ� �� �ؽ�Ʈ �ʱ�ȭ
            if (waitTimerText != null)
                waitTimerText.text = "";
        }
    }
}
