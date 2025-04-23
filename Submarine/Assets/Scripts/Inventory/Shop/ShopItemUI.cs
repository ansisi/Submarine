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
    public TextMeshProUGUI buttonText;
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
        priceText.text = $"{data.buyPrice}$";
        quantityText.gameObject.SetActive(false);
        buttonText.text = "Buy";
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
        priceText.text = $"{data.sellPrice}$";
        quantityText.text = $"x{quantity}";
        quantityText.gameObject.SetActive(true);
        buttonText.text = "Sell";
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(SellItem);
    }

    private void BuyItem()
    {
        if (CurrencyManager.Instance.gold >= shopItemData.buyPrice)
        {
            CurrencyManager.Instance.SpendGold(shopItemData.buyPrice);
            InventoryManager.Instance.AddItem(shopItemData.item, 1);
            Logger.Log($"[상점] {shopItemData.item.itemName} 구매 완료");
        }
        else
        {
            Logger.Log("골드가 부족합니다.");
        }
    }

    private void SellItem()
    {
        if (InventoryManager.Instance.CountItem(shopItemData.item) > 0)
        {
            bool success = InventoryManager.Instance.RemoveItem(shopItemData.item, 1);
            if (success)
            {
                CurrencyManager.Instance.AddGold(shopItemData.sellPrice);
                Logger.Log($"[상점] {shopItemData.item.itemName} 1개 판매 완료 (+{shopItemData.sellPrice}G)");

                // 수량 갱신
                sellQuantity--;
                quantityText.text = $"x{sellQuantity}";

                // 수량이 0 이하가 되면 버튼 비활성화
                if (sellQuantity <= 0)
                {
                    actionButton.interactable = false;
                    buttonText.text = "Sold Out";
                }
            }
        }
        else
        {
            Logger.Log("판매할 아이템이 없습니다.");
        }
    }
}
