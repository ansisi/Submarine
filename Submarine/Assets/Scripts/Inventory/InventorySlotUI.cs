using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;          // 아이템 아이콘 이미지
    public TextMeshProUGUI quantityText;   // 아이템 개수 텍스트

    // 슬롯에 표시되는 내용을 갱신하는 함수
    public void UpdateSlotUI(Item item, int quantity)
    {
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
    }
}
