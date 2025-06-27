using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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
        if (skillImage != null)
            skillImage.enabled = false; // ó���� ����
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
        }
    }

    IEnumerator RollingLoopRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitInterval);

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

            int result = Random.Range(3, 4);
            currentDiceResult = result;
            image.sprite = diceSprites[result - 1];
            Debug.Log($"�ֻ��� ���: {result}");

            // ����� �´� ��ų �̹��� ǥ��
            if (skillImage != null && skillSprites != null && skillSprites.Count >= 4)
            {
                skillImage.sprite = skillSprites[result - 1];
                skillImage.enabled = true;  // ��ų �̹��� ���̰�
            }

            isRolling = false;
            if (joystickCanvasGroup != null)
                joystickCanvasGroup.alpha = 1f;
        }
    }
}
