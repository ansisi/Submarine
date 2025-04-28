using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellSlotUI : MonoBehaviour
{
    public Item Item { get; private set; }
    public int Quantity { get; private set; }
    public int SellPrice { get; private set; }

    [Header("UI Components")]
    public Image icon;
    public TextMeshProUGUI quantityText;
    public Button button;

    public void Setup(Item item, int quantity)
    {
        Item = item;
        Quantity = quantity;
        SellPrice = FindSellPrice(item);

        icon.sprite = item.icon;
        quantityText.text = $"x{Quantity}";

        button.onClick.RemoveAllListeners(); // 혹시 모르니까 기존 리스너 제거
        button.onClick.AddListener(OnClickSlot); // 여기서 코드로 연결!
    }

    public void AddQuantity(int amount)
    {
        Quantity += amount;
        quantityText.text = $"x{Quantity}";
    }

    public void ReduceQuantity(int amount)
    {
        Quantity -= amount;
        quantityText.text = $"x{Quantity}";
    }

    private int FindSellPrice(Item item)
    {
        return item.sellPrice;
    }

    // 슬롯을 클릭했을 때 실행할 함수
    public void OnClickSlot()
    {
        SellManager.Instance.RemoveItemFromSell(this);
        InventoryManager.Instance.AddItem(Item, 1);  // 인벤토리로 다시 되돌리기
    }
}
