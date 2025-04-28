using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellSlotUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI quantityText;
    private int slotIndex; // SellManager에서 관리하는 슬롯 번호

    private SellSlot currentSlot;

    public void Setup(int index)
    {
        slotIndex = index;
        GetComponent<Button>().onClick.AddListener(OnClickSlot);
    }

    public void UpdateSlotUI(SellSlot slot)
    {
        currentSlot = slot;

        if (slot != null && !slot.IsEmpty)
        {
            icon.sprite = slot.item.icon;
            icon.enabled = true;
            quantityText.text = $"x{slot.quantity}";
        }
        else
        {
            icon.enabled = false;
            quantityText.text = "";
        }
    }

    private void OnClickSlot()
    {
        if (currentSlot != null && !currentSlot.IsEmpty)
        {
            InventoryManager.Instance.AddItem(currentSlot.item, currentSlot.quantity);
            SellManager.Instance.RemoveItemFromSell(currentSlot);
        }
    }
}
