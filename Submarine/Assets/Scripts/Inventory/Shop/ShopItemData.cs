using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Item", menuName = "Shop/Shop Item")]
public class ShopItemData : ScriptableObject
{
    public Item item;             // 실제 아이템 참조 (인벤토리 시스템과 공유)
    public int buyPrice;          // 구매 가격 (골드 차감)
    public int sellPrice;         // 판매 가격 (골드 획득)
    public int stock = -1;        // 재고 수량 (-1이면 무제한)

    public string description;
}
