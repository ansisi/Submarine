using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMaterialSlot : MonoBehaviour
{
    public Image icon;
    public TMP_Text amountText;

    private UpgradeMaterialRequirement currentReq;

    public void Set(UpgradeMaterialRequirement req)
    {
        currentReq = req;
        icon.sprite = req.item.icon;
        int have = InventoryManager.Instance.GetItemCount(req.item);
        amountText.text = $"{have}/{req.amount}";
    }

    public void Clear()
    {
        icon.sprite = null;
        amountText.text = "";
    }
}