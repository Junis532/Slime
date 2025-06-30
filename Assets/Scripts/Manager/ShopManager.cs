using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

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

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        rerollPriceText.text = $"리롤 {rerollPrice}원";
        rerollButton.onClick.AddListener(RerollItems);
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

            // 리롤 시 모든 버튼 다시 활성화
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

            // 리롤 시 모든 버튼 다시 활성화
            buyBtn.interactable = true;
        }

        rerollPriceText.text = $"리롤 {rerollPrice}원";
        UpdateRerollButtonState();
    }

    void BuyItem(ItemStats item)
    {
        Debug.Log($"[구매] {item.itemName} - 돈 차감 없음");

        // ★ 아이템별 효과 처리 ★
        if (item == GameManager.Instance.itemStats1)
        {
            Debug.Log("아이템1 효과 발동!");
            GameManager.Instance.playerStats.maxHP += 5;
            GameManager.Instance.playerStats.currentHP += 5;
        }
        else if (item == GameManager.Instance.itemStats2)
        {
            Debug.Log("아이템2 효과 발동!");
            // 효과 구현 가능
        }
        else if (item == GameManager.Instance.itemStats3)
        {
            Debug.Log("아이템3 효과 발동!");
            GameManager.Instance.playerStats.speed *= 1.03f;
        }
        else if (item == GameManager.Instance.itemStats4)
        {
            Debug.Log("아이템4 효과 발동!");
            // 효과 구현 가능
        }
        else if (item == GameManager.Instance.itemStats5)
        {
            Debug.Log("아이템5 효과 발동!");
            // 효과 구현 가능
        }
        else if (item == GameManager.Instance.itemStats6)
        {
            Debug.Log("아이템6 효과 발동!");
            // 효과 구현 가능
        }

        // 구매한 슬롯 버튼 외에 모두 비활성화
        foreach (GameObject slot in itemSlots)
        {
            TextMeshProUGUI nameText = slot.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            Button buyBtn = slot.transform.Find("BuyButton").GetComponent<Button>();

            if (nameText.text == item.itemName)
                buyBtn.interactable = false;
            else
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
        // 현재는 비활성화 상태이므로 빈 구현
    }

    public void OnButtonNextWaveClick()
    {
        GameManager.Instance.waveManager.StartNextWave();
    }
}
