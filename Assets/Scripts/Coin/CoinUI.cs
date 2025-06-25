using UnityEngine;
using TMPro;

public class CoinUI : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI shopCoinText; // �߰��� �ؽ�Ʈ �ʵ�

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.playerStats != null)
        {
            coinText.text = $"{GameManager.Instance.playerStats.coin}";
            shopCoinText.text = $"{GameManager.Instance.playerStats.coin}";
        }
    }
}
