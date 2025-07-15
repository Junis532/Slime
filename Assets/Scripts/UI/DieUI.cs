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
        retryButton.onClick.AddListener(OnRetryClicked);
    }

    void OnLeaveClicked()
    {
        Application.Quit();
    }

    void OnRetryClicked()
    {
        LoadingManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
