using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUIButton : MonoBehaviour
{
    public GameObject craftingUIPanel;
    public GameObject craftingDetailUIPanel;
    public Button openButton; // 버튼 연결

    public CraftingRecipeUIManager craftingRecipeUIManager;

    private void Start()
    {
        if (openButton != null)
            openButton.onClick.AddListener(OpenCraftingUI);
    }

    private void OpenCraftingUI()
    {
        if (craftingUIPanel != null)
        {
            craftingUIPanel.SetActive(true);
            craftingRecipeUIManager.Initialize();
        }

        if(craftingDetailUIPanel != null)
        {
            craftingDetailUIPanel.SetActive(true);
        }
            
    }
}
