using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; // 싱글톤 인스턴스 (다른 클래스에서 접근 가능하게)

    public int slotCount = 20;        // 인벤토리 슬롯 개수
    public List<InventorySlot> slots = new List<InventorySlot>(); // 슬롯 리스트

    public List<InventorySlot> GetAllSlots()
    {
        return slots;
    }

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
            Instance = this;

        slotCount = 10; // 초기 슬롯 개수 설정
        slots.Clear(); // 추가

        // 슬롯 초기화
        for (int i = 0; i < slotCount; i++)
        {
            slots.Add(new InventorySlot());
        }
    }

    // 아이템을 인벤토리에 추가하는 함수
    public bool AddItem(Item item, int quantity = 1)
    {

        // 스택 불가 아이템은 무조건 1개만 추가
        if (!item.isStackable)
        {
            quantity = 1;
        }

        // 중첩 가능한 아이템이면 기존 슬롯에 추가 시도
        if (item.isStackable)
        {
            foreach (var slot in slots)
            {
                if (!slot.IsEmpty && slot.item == item)
                {
                    slot.quantity += quantity;
                    InventoryUIManager.instance?.UpdateUI(); // UI 갱신 추가
                    return true;
                }
            }
        }

        // 비어있는 슬롯에 새로 추가
        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
            {
                slot.AddItem(item, quantity);
                InventoryUIManager.instance?.UpdateUI(); // UI 갱신 추가
                return true;
            }
        }

        // 슬롯이 다 차면 실패
        Logger.Log("인벤토리가 가득 찼습니다.");
        return false;
    }

    // 특정 아이템의 개수를 세는 함수
    public int CountItem(Item item)
    {
        int total = 0;
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty && slot.item == item)
            {
                total += slot.quantity;
            }
        }
        return total;
    }

    // 특정 아이템을 제거하는 함수
    public bool RemoveItem(Item item, int quantity)
    {
        int remaining = quantity;

        foreach (var slot in slots)
        {
            if (!slot.IsEmpty && slot.item == item)
            {
                if (slot.quantity >= remaining)
                {
                    slot.quantity -= remaining;
                    if (slot.quantity == 0) slot.ClearSlot();
                    InventoryUIManager.instance?.UpdateUI();
                    return true;
                }
                else
                {
                    remaining -= slot.quantity;
                    slot.ClearSlot();
                }
            }
        }

        InventoryUIManager.instance?.UpdateUI();
        return false; // 충분한 수량이 없어서 실패
    }

    // 특정 아이템의 개수를 반환하는 함수
    public bool HasEnoughItems(List<UpgradeMaterialRequirement> requirements)
    {
        foreach (var req in requirements)
        {
            if (CountItem(req.item) < req.amount)
                return false;
        }
        return true;
    }

    // 업그레이드 재료를 소비하는 함수
    public void ConsumeItems(List<UpgradeMaterialRequirement> requirements)
    {
        foreach (var req in requirements)
        {
            RemoveItem(req.item, req.amount);
        }
    }

    public void UpgradeSlotCount(int newCount)
    {
        if (newCount <= slotCount)
        {
            Logger.Log("슬롯 수 증가가 필요하지 않음 (이미 같은 수나 더 많음)");
            return;
        }

        int added = newCount - slotCount;
        slotCount = newCount;

        for (int i = 0; i < added; i++)
        {
            slots.Add(new InventorySlot());
        }

        Logger.Log($"인벤토리 슬롯이 {slotCount}칸으로 업그레이드됨");
        InventoryUIManager.instance?.ExpandSlotUI(added);
        InventoryUIManager.instance?.UpdateUI();
    }
    
    // 특정 아이템이 인벤토리에 있는지 확인하는 함수
    public bool HasItem(Item item, int amount)
    {
        return CountItem(item) >= amount;
    }

    // 특정 아이템의 개수를 반환하는 함수
    public int GetItemCount(Item item)
    {
        return CountItem(item);
    }


}
