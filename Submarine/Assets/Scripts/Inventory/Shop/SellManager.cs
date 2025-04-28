using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellManager : MonoBehaviour
{
    public static SellManager Instance;

    public int slotCount = 10; // 고정 슬롯 수
    public List<SellSlot> slots = new List<SellSlot>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        slots.Clear();
        for (int i = 0; i < slotCount; i++)
        {
            slots.Add(new SellSlot());
        }
    }

    public List<SellSlot> GetAllSlots()
    {
        return slots;
    }

    public bool AddItemToSell(Item item, int quantity = 1)
    {
        // 먼저 같은 아이템이 있는 슬롯 찾기
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty && slot.item == item)
            {
                slot.quantity += quantity;
                Logger.Log($"[판매창] {item.itemName} 수량 추가됨 - 현재 수량: {slot.quantity}");
                SellUIManager.Instance?.UpdateUI();
                return true;
            }
        }

        // 없다면 빈 슬롯에 추가
        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
            {
                slot.SetItem(item, quantity);
                Logger.Log($"[판매창] {item.itemName} 새 슬롯 추가 - 수량: {quantity}");
                SellUIManager.Instance?.UpdateUI();
                return true;
            }
        }

        Logger.Log("판매창에 빈 슬롯이 없습니다.");
        return false;
    }

    public void RemoveItemFromSell(SellSlot slot)
    {
        slot.ClearSlot();
        SellUIManager.Instance?.UpdateUI();
    }

    public void SellAllItems()
    {
        int totalGold = 0;
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty)
            {
                int price = slot.item.sellPrice * slot.quantity;
                totalGold += price;
                Logger.Log($"[판매] {slot.item.itemName} {slot.quantity}개 판매 완료 (+{price}G)");

                slot.ClearSlot(); // 판매 후 슬롯 비우기
            }
        }

        if (totalGold > 0)
        {
            CurrencyManager.Instance.AddGold(totalGold);
        }

        SellUIManager.Instance?.UpdateUI();
    }
}

[System.Serializable]
public class SellSlot
{
    public Item item;
    public int quantity;

    public bool IsEmpty => item == null;

    public void SetItem(Item newItem, int newQuantity)
    {
        item = newItem;
        quantity = newQuantity;
    }

    public void ClearSlot()
    {
        item = null;
        quantity = 0;
    }
}