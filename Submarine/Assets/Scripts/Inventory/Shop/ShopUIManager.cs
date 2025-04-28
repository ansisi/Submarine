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
            openButton.onClick.AddListener(OpenShoppingUI);
    }

    private void OpenShoppingUI()
    {
        if (ShopUIPanel != null)
            ShopUIPanel.SetActive(true);

        if (SellPanel != null)
            SellPanel.SetActive(true);

        ShopManager.Instance.OpenShop(); // ShopManager 쪽도 열림 처리
    }

}
