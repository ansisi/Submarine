using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public Item item;        // 현재 슬롯에 들어 있는 아이템
    public int quantity;     // 아이템 개수

    // 슬롯이 비었는지 확인하는 프로퍼티
    public bool IsEmpty => item == null || quantity <= 0;

    // 아이템 추가 함수
    public void AddItem(Item newItem, int amount)
    {
        item = newItem;
        quantity += amount;
    }

    // 슬롯 비우는 함수
    public void ClearSlot()
    {
        item = null;
        quantity = 0;
    }
}
