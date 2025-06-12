using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUIButton : MonoBehaviour
{
    public GameObject craftingUIPanel;
    public GameObject craftingDetailUIPanel;
    public Button openButton;

    public CraftingRecipeUIManager craftingRecipeUIManager;
    public bool IsPanelOpen => craftingUIPanel != null && craftingUIPanel.activeSelf;

    private void Start()
    {
        if (openButton != null)
            openButton.onClick.AddListener(ToggleCraftingUI);
    }

    private void ToggleCraftingUI()
    {
        var upgradeUI = FindObjectOfType<UpgradeUIManager>();
        if (upgradeUI != null && upgradeUI.IsPanelOpen)
        {
            // (선택) 안내 메시지 띄우기
            return;
        }

        if (craftingUIPanel == null) return;

        bool isActive = craftingUIPanel.activeSelf;

        craftingUIPanel.SetActive(!isActive);
        craftingDetailUIPanel?.SetActive(!isActive);

        if (!isActive)
        {
            // UI를 켤 때만 초기화 실행
            craftingRecipeUIManager?.Initialize();
        }
    }
}
