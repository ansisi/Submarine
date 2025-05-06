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

    private Transform playerTransform;

    private void Start()
    {
        // 플레이어를 찾는다 (PlayerMovement를 쓰고 있다고 가정)
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

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
        if (item == null) return;

        if (ShopManager.Instance.IsShopOpen)
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
        else
        {
            if (item.isConsumable)
            {
                ConsumeItem();
            }
            else
            {
                TryPlaceItem();
            }
        }

    }

    private void TryPlaceItem()
    {
        if (item.isPlaceable && item.prefab != null && playerTransform != null)
        {
            Vector3 placePosition = playerTransform.position + Vector3.up * 1.5f;
            Instantiate(item.prefab, placePosition, Quaternion.identity);

            bool removed = InventoryManager.Instance.RemoveItem(item, 1);
            if (!removed)
            {
                Logger.Log("인벤토리에 아이템이 부족해서 설치 실패");
            }

            InventoryUIManager.instance?.UpdateUI();
        }
        else
        {
            Logger.Log($"[{item.itemName}] 설치할 수 없는 아이템이거나 플레이어를 찾지 못했음");
        }
    }

    private void ConsumeItem()
    {
        switch (item.itemName)
        {
            case "물":
            case "Water":
                WaterTank.Instance?.AddWater(30f);
                break;

            // 다른 소모품 추가 가능
            // case "산소통":
            //     OxygenTank.Instance?.AddOxygen(50f);
            //     break;

            default:
                Logger.Log($"[{item.itemName}] 는 사용 가능한 소모품이 아님");
                return;
        }

        bool removed = InventoryManager.Instance.RemoveItem(item, 1);
        if (!removed)
        {
            Logger.Log("인벤토리에 아이템이 부족해서 사용 실패");
        }
    }

}
