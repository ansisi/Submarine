using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public ShopItemData[] itemsForSale;
    public GameObject itemUIPrefab;
    public Transform itemListParent;

    public bool isBuying = true;

    private void Start()
    {
        RefreshShopUI();
    }

    public void RefreshShopUI()
    {
        foreach (Transform child in itemListParent)
            Destroy(child.gameObject);

        if (isBuying)
        {
            foreach (ShopItemData item in itemsForSale)
            {
                GameObject go = Instantiate(itemUIPrefab, itemListParent);
                go.GetComponent<ShopItemUI>().SetItemForBuy(item);
            }
        }
        else
        {
            foreach (InventorySlot slot in InventoryManager.instance.GetAllSlots())
            {
                if (slot.item == null) continue;

                ShopItemData shopData = FindShopDataByItem(slot.item);
                if (shopData == null) continue;

                GameObject go = Instantiate(itemUIPrefab, itemListParent);
                go.GetComponent<ShopItemUI>().SetItemForSell(shopData, slot.quantity);
            }
        }
    }

    public void ToggleMode(bool buyMode)
    {
        isBuying = buyMode;
        RefreshShopUI();
    }

    private ShopItemData FindShopDataByItem(Item item)
    {
        foreach (ShopItemData data in itemsForSale)
        {
            if (data.item == item)
                return data;
        }
        return null;
    }
}
