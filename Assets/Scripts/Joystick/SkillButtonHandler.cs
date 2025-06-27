using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public JoystickDirectionIndicator3 directionIndicator;
    public CanvasGroup joystickCanvasGroup; // ���̽�ƽ ���� ������
    private Image skillImage;

    private void Start()
    {
        skillImage = GetComponent<Image>();

        if (joystickCanvasGroup != null)
        {
            joystickCanvasGroup.alpha = 0f;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        directionIndicator.OnSkillButtonPressed();

        if (skillImage != null)
            skillImage.enabled = false;

        if (joystickCanvasGroup != null)
            joystickCanvasGroup.alpha = 1f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        directionIndicator.OnSkillButtonReleased();

        if (skillImage != null)
            skillImage.enabled = true;

        if (joystickCanvasGroup != null)
            joystickCanvasGroup.alpha = 0f;
        Debug.Log("��ų ��ư �������!");
    }
}
