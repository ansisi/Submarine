using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialItemUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image materialIcon;  // 재료 아이콘
    public TextMeshProUGUI materialNameText;  // 재료 이름
    public TextMeshProUGUI materialAmountText;  // 재료 수량

    private UpgradeMaterialRequirement materialData;

    // 재료 데이터 설정
    public void SetMaterialData(UpgradeMaterialRequirement data)
    {
        materialData = data;
        materialIcon.sprite = data.item.icon;
        materialNameText.text = data.item.itemName;
        materialAmountText.text = "x" + data.amount;
    }
}