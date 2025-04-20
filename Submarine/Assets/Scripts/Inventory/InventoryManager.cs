using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance; // 싱글톤 인스턴스 (다른 클래스에서 접근 가능하게)

    public int slotCount = 20;        // 인벤토리 슬롯 개수
    public List<InventorySlot> slots = new List<InventorySlot>(); // 슬롯 리스트

    public List<InventorySlot> GetAllSlots()
    {
        return slots;
    }

    private void Awake()
    {
        // 싱글톤 설정
        if (instance == null)
            instance = this;

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
                    InventoryUIManger.instance?.UpdateUI(); // UI 갱신 추가
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
                InventoryUIManger.instance?.UpdateUI(); // UI 갱신 추가
                return true;
            }
        }

        // 슬롯이 다 차면 실패
        Logger.Log("인벤토리가 가득 찼습니다.");
        return false;
    }

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
                    InventoryUIManger.instance?.UpdateUI();
                    return true;
                }
                else
                {
                    remaining -= slot.quantity;
                    slot.ClearSlot();
                }
            }
        }

        InventoryUIManger.instance?.UpdateUI();
        return false; // 충분한 수량이 없어서 실패
    }

}
