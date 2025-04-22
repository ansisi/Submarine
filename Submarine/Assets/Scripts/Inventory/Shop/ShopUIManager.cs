using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    public GameObject ShopUIPanel;
    public Button openButton;

    private void Start()
    {
        if (openButton != null)
            openButton.onClick.AddListener(OpenShoppingUI);
    }

    private void OpenShoppingUI()
    {
        if (ShopUIPanel != null)
        {
            ShopUIPanel.SetActive(true);
        }
    }

}
