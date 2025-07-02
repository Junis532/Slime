using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; // Ensure you have the TextMeshPro package installed if using TMPro

public class LoadingTextAnimation : MonoBehaviour
{
    public TMP_Text loadingText;        // UI Text ������Ʈ
    public string baseText = "Loading.";
    public float dotInterval = 0.5f;  // �� �ϳ� �߰� ���� (��)

    private int dotCount = 1;
    private int maxDots = 2;

    void Start()
    {
        if (loadingText == null)
            loadingText = GetComponent<TMP_Text>();

        StartCoroutine(AnimateLoadingDots());
    }

    IEnumerator AnimateLoadingDots()
    {
        while (true)
        {
            dotCount = (dotCount + 1) % (maxDots + 1);  // 0,1,2,3 �ݺ�

            loadingText.text = baseText + new string('.', dotCount);

            yield return new WaitForSeconds(dotInterval);
        }
    }
}
