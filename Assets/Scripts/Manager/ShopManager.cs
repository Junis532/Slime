using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("아이템 데이터")]
    public List<ItemStats> allItems;

    [Header("UI 슬롯 (6개)")]
    public List<GameObject> itemSlots;

    [Header("버튼")]
    public Button rerollButton;
    public Button nextWaveButton;

    [Header("리롤 가격")]
    public TextMeshProUGUI rerollPriceText;
    public int rerollPrice = 1;

    [Header("상점 패널")]
    public RectTransform shopPanel;

    [Header("상점 UI 오브젝트")]
    public GameObject shopUI;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        rerollPriceText.text = $"리롤 {rerollPrice}원";
        rerollButton.onClick.AddListener(RerollItems);
        nextWaveButton.onClick.AddListener(OnButtonNextWaveClick);
        RerollItems();
    }

    public void InitShopUI()
    {
        UpdateRerollButtonState();
        UpdateBuyButtonStates();
    }

    public void ResetRerollPrice()
    {
        rerollPrice = 1; // 초기값으로 리셋
        rerollPriceText.text = $"리롤 {rerollPrice}원";
        UpdateRerollButtonState();
    }

    public void RerollItems()
    {
        int coin = GameManager.Instance.playerStats.coin;

        if (coin < rerollPrice)
        {
            Debug.Log("코인이 부족하여 리롤할 수 없습니다!");
            return;
        }

        GameManager.Instance.playerStats.coin -= rerollPrice;
        rerollPrice *= 2; // 리롤 가격 증가

        List<ItemStats> selectedItems = GetRandomItems(itemSlots.Count);

        for (int i = 0; i < itemSlots.Count; i++)
        {
            GameObject slot = itemSlots[i];
            ItemStats item = selectedItems[i];

            slot.transform.Find("ItemName").GetComponent<TextMeshProUGUI>().text = item.itemName;
            slot.transform.Find("ItemDescription").GetComponent<TextMeshProUGUI>().text = item.description;
            slot.transform.Find("ItemPrice").GetComponent<TextMeshProUGUI>().text = item.price.ToString();
            slot.transform.Find("ItemIcon").GetComponent<Image>().sprite = item.icon;

            Button buyBtn = slot.transform.Find("BuyButton").GetComponent<Button>();
            buyBtn.onClick.RemoveAllListeners();

            ItemStats capturedItem = item;
            buyBtn.onClick.AddListener(() => BuyItem(capturedItem));

            buyBtn.interactable = true;
        }

        rerollPriceText.text = $"리롤 {rerollPrice}원";
        UpdateRerollButtonState();
    }

    public void FirstRerollItems()
    {
        List<ItemStats> selectedItems = GetRandomItems(itemSlots.Count);

        for (int i = 0; i < itemSlots.Count; i++)
        {
            GameObject slot = itemSlots[i];
            ItemStats item = selectedItems[i];

            slot.transform.Find("ItemName").GetComponent<TextMeshProUGUI>().text = item.itemName;
            slot.transform.Find("ItemDescription").GetComponent<TextMeshProUGUI>().text = item.description;
            slot.transform.Find("ItemPrice").GetComponent<TextMeshProUGUI>().text = item.price.ToString();
            slot.transform.Find("ItemIcon").GetComponent<Image>().sprite = item.icon;

            Button buyBtn = slot.transform.Find("BuyButton").GetComponent<Button>();
            buyBtn.onClick.RemoveAllListeners();

            ItemStats capturedItem = item;
            buyBtn.onClick.AddListener(() => BuyItem(capturedItem));

            buyBtn.interactable = true;
        }

        rerollPriceText.text = $"리롤 {rerollPrice}원";
        UpdateRerollButtonState();
    }

    void BuyItem(ItemStats item)
    {
        Debug.Log($"[구매] {item.itemName} - 돈 차감 없음");

        // 아이템 효과 발동 (기존 코드 유지)
        if (item == GameManager.Instance.itemStats1)
        {
            GameManager.Instance.playerStats.maxHP += 5;
            GameManager.Instance.playerStats.currentHP += 5;
        }
        else if (item == GameManager.Instance.itemStats2)
        {
            GameObject gmObj = GameManager.Instance.gameObject;
            PoisonSpawner poisonSpawner = gmObj.GetComponent<PoisonSpawner>();

            if (poisonSpawner != null)
            {
                if (!poisonSpawner.enabled)
                {
                    poisonSpawner.enabled = true;
                }
                else
                {
                    poisonSpawner.poisonLifetime += 1;
                }
            }
        }
        else if (item == GameManager.Instance.itemStats3)
        {
            GameManager.Instance.playerStats.speed *= 1.03f;
        }
        else if (item == GameManager.Instance.itemStats4)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                var meteorSkill = playerObj.GetComponent<MeteorOrbitSkill>();
                if (meteorSkill != null)
                {
                    if (!meteorSkill.enabled)
                    {
                        meteorSkill.enabled = true;
                        meteorSkill.meteorCount = 1;
                    }
                    else
                    {
                        if (meteorSkill.meteorCount < 4)
                            meteorSkill.meteorCount += 1;
                        else
                            meteorSkill.rotationSpeed += 20;

                        meteorSkill.RefreshMeteor();
                    }
                }
            }
        }
        else if (item == GameManager.Instance.itemStats5)
        {
            GameManager.Instance.playerStats.attack *= 1.02f;
        }
        else if (item == GameManager.Instance.itemStats6)
        {
            GameObject gmObj = GameManager.Instance.gameObject;
            BulletSpawner spawner = gmObj.GetComponent<BulletSpawner>();
            if (spawner != null)
            {
                spawner.bulletCount += 1;
            }
        }

        // 구매 후 모든 버튼 비활성화 (모두 비활성화)
        foreach (GameObject slot in itemSlots)
        {
            Button buyBtn = slot.transform.Find("BuyButton").GetComponent<Button>();
            buyBtn.interactable = false;
        }

        UpdateRerollButtonState();
        UpdateBuyButtonStates();
    }

    List<ItemStats> GetRandomItems(int count)
    {
        List<ItemStats> copy = new List<ItemStats>(allItems);
        List<ItemStats> result = new List<ItemStats>();

        for (int i = 0; i < count && copy.Count > 0; i++)
        {
            int idx = Random.Range(0, copy.Count);
            result.Add(copy[idx]);
            copy.RemoveAt(idx);
        }
        return result;
    }

    void UpdateRerollButtonState()
    {
        int coin = GameManager.Instance.playerStats.coin;
        rerollButton.interactable = coin >= rerollPrice;
    }

    void UpdateBuyButtonStates()
    {
        // 빈 구현, 필요시 추가
    }

    public void OnButtonNextWaveClick()
    {
        Debug.Log("다음 웨이브 시작");
        Time.timeScale = 1f;

        if (shopPanel != null)
        {
            shopPanel.DOKill();
            CanvasGroup canvasGroup = shopPanel.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.DOFade(0f, 0.7f);  // 0f = 완전 투명, 0.5초 동안
            }
            shopPanel.DOAnchorPosY(1500f, 0.7f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                if (shopUI != null)
                    shopUI.SetActive(false);
            });
        }
        else
        {
            if (shopUI != null)
                shopUI.SetActive(false);
        }

        GameManager.Instance.waveManager.StartNextWave();
    }
}
