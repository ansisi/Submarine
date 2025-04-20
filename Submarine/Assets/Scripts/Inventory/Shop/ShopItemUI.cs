using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI quantityText;
    public Button actionButton;

    private ShopItemData shopItemData;
    private bool isBuyMode;
    private int sellQuantity;

    public void SetItemForBuy(ShopItemData data)
    {
        shopItemData = data;
        isBuyMode = true;
        icon.sprite = data.item.icon;
        nameText.text = data.item.itemName;
        priceText.text = $"{data.buyPrice}G";
        quantityText.gameObject.SetActive(false);
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(BuyItem);
    }

    public void SetItemForSell(ShopItemData data, int quantity)
    {
        shopItemData = data;
        isBuyMode = false;
        sellQuantity = quantity;
        icon.sprite = data.item.icon;
        nameText.text = data.item.itemName;
        priceText.text = $"{data.sellPrice}G";
        quantityText.text = $"x{quantity}";
        quantityText.gameObject.SetActive(true);
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(SellItem);
    }

    private void BuyItem()
    {
        if (CurrencyManager.Instance.gold >= shopItemData.buyPrice)
        {
            CurrencyManager.Instance.SpendGold(shopItemData.buyPrice);
            InventoryManager.instance.AddItem(shopItemData.item, 1);
            Debug.Log($"[상점] {shopItemData.item.itemName} 구매 완료");
        }
        else
        {
            Debug.Log("골드가 부족합니다.");
        }
    }

    private void SellItem()
    {
        if (InventoryManager.instance.CountItem(shopItemData.item) > 0)
        {
            bool success = InventoryManager.instance.RemoveItem(shopItemData.item, 1);
            if (success)
            {
                CurrencyManager.Instance.AddGold(shopItemData.sellPrice);
                Debug.Log($"[상점] {shopItemData.item.itemName} 1개 판매 완료 (+{shopItemData.sellPrice}G)");
            }
        }
        else
        {
            Debug.Log("판매할 아이템이 없습니다.");
        }
    }
}
