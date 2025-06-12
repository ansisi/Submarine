using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    public GameObject ShopUIPanel;
    public GameObject SellPanel;
    public Button openButton;


    private void Start()
    {
        if (openButton != null)
            openButton.onClick.AddListener(ToggleShoppingUI);
    }

    private void ToggleShoppingUI()
    {
        if (ShopUIPanel == null) return;

        bool isActive = ShopUIPanel.activeSelf;

        ShopUIPanel.SetActive(!isActive);
        SellPanel?.SetActive(!isActive);

        if (!isActive)
        {
            ShopManager.Instance?.OpenShop();

            if (InventoryUIController.instance != null && !InventoryUIController.instance.isOpen)
            {
                InventoryUIController.instance.ToggleInventory();
            }
        }
        else
        {
            if (InventoryUIController.instance != null && InventoryUIController.instance.isOpen)
            {
                InventoryUIController.instance.ToggleInventory();
            }
        }
    }

}
