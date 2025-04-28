using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    public Item[] itemsForSale;
    public GameObject itemUIPrefab;
    public Transform itemListParent;

    public bool IsShopOpen { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RefreshShopUI()
    {
        foreach (Transform child in itemListParent)
            Destroy(child.gameObject);

        foreach (Item item in itemsForSale)
        {
            if (!item.isPurchasable) continue;

            GameObject go = Instantiate(itemUIPrefab, itemListParent);
            go.GetComponent<ShopItemUI>().SetItemForBuy(item);
        }
    }

    public void OpenShop()
    {
        IsShopOpen = true;
        // UI ÄÑ±â
        RefreshShopUI();
    }

    public void CloseShop()
    {
        IsShopOpen = false;
        // UI ²ô±â
    }

}
