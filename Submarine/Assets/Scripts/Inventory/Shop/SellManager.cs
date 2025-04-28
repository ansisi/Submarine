using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellManager : MonoBehaviour
{
    public static SellManager Instance;

    public GameObject sellSlotPrefab; // 판매 창에 들어갈 슬롯 프리팹
    public Transform sellSlotParent;  // 슬롯들이 들어갈 부모 오브젝트

    private List<SellSlotUI> sellSlots = new List<SellSlotUI>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // 인벤토리에서 아이템을 판매창으로 추가
    public void AddItemToSell(Item item, int quantity)
    {
        // 이미 판매창에 있는지 체크
        SellSlotUI slot = sellSlots.Find(s => s.Item == item);

        if (slot != null)
        {
            slot.AddQuantity(1); // 수량 +1
        }
        else
        {
            GameObject go = Instantiate(sellSlotPrefab, sellSlotParent);
            SellSlotUI newSlot = go.GetComponent<SellSlotUI>();
            newSlot.Setup(item, 1);
            sellSlots.Add(newSlot);
        }
    }

    // 판매창에서 아이템 제거 (수량 줄이거나 아예 없애기)
    public void RemoveItemFromSell(SellSlotUI slot)
    {
        slot.ReduceQuantity(1);

        if (slot.Quantity <= 0)
        {
            sellSlots.Remove(slot);
            Destroy(slot.gameObject);
        }
    }

    // 판매 버튼 눌렀을 때
    public void SellAllItems()
    {
        foreach (var slot in sellSlots)
        {
            if (InventoryManager.Instance.RemoveItem(slot.Item, slot.Quantity))
            {
                int totalPrice = slot.Quantity * slot.SellPrice;
                CurrencyManager.Instance.AddGold(totalPrice);
                Logger.Log($"[판매] {slot.Item.itemName} {slot.Quantity}개 판매 완료 (+{totalPrice}G)");
            }
        }

        // 판매 완료 후 창 비우기
        foreach (var slot in sellSlots)
        {
            Destroy(slot.gameObject);
        }
        sellSlots.Clear();
    }
}