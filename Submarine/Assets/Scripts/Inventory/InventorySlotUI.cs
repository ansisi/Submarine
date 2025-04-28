using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;          // 아이템 아이콘 이미지
    public TextMeshProUGUI quantityText;   // 아이템 개수 텍스트
    public Button button;

    private Item item;
    private int quantity;

    // 슬롯에 표시되는 내용을 갱신하는 함수
    public void UpdateSlotUI(Item newItem, int newQuantity)
    {
        item = newItem;
        quantity = newQuantity;

        if (item != null)
        {
            icon.sprite = item.icon;
            icon.enabled = true;
            quantityText.text = item.isStackable ? quantity.ToString() : "";
        }
        else
        {
            icon.enabled = false;
            quantityText.text = "";
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClickSlot);
    }

    private void OnClickSlot()
    {
        if (item != null && ShopManager.Instance.IsShopOpen)
        {
            Debug.Log($"[DEBUG] 인벤토리 슬롯 클릭됨: {item.itemName}");
            SellManager.Instance.AddItemToSell(item, 1);
        }
    }

}
