using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    static string nextScene;

    [SerializeField]
    Image progressBar;

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("Loading");
    }

    void Start()
    {
        StartCoroutine(LoadSceneProgress());
    }

    IEnumerator LoadSceneProgress()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0f;

        while (!op.isDone)
        {
            yield return null;

            if (op.progress < 0.9f)
            {
                timer += Time.unscaledDeltaTime;
                progressBar.fillAmount = Mathf.Clamp01(timer / 2f); // 2초 동안 로딩바 채우기
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer); // 0.9~1f 구간 보정

                if (progressBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true; // 씬 전환 허용
                }
            }
        }
    }
}
