using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DieUI : MonoBehaviour
{
    public Button leaveButton;
    public Button retryButton;

    void Start()
    {
        leaveButton.onClick.AddListener(OnLeaveClicked);
    }

    void OnLeaveClicked()
    {
        Application.Quit();
    }

}
