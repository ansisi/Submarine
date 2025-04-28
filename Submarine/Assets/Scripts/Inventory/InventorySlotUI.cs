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
            Logger.Log($"[인벤토리] {item.itemName} 클릭됨 - 인벤토리 수량: {quantity}");

            Item clickedItem = item;

            bool removed = InventoryManager.Instance.RemoveItem(item, 1); // 인벤토리 수량 1개 차감
            if (removed)
            {
                SellManager.Instance.AddItemToSell(clickedItem, 1);
            }
            else
            {
                Logger.Log("인벤토리에 아이템이 부족해서 판매창에 추가 못함");
            }

            InventoryUIManager.instance?.UpdateUI(); // 인벤토리 UI 갱신
        }
    }

}
