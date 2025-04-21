using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    public static GameUIController Instance;

    [SerializeField] private GameObject npcUpgradeButton;
    [SerializeField] private GameObject npcUpgradePanel; // 업그레이드 UI 패널

    private void Awake()
    {
        Instance = this;
    }

    public void UnlockNPCUpgradeUI()
    {
        if (npcUpgradeButton != null)
        {
            npcUpgradeButton.SetActive(true);
        }
    }

    // 버튼 클릭 시 호출
    public void OnNPCUpgradeButtonClick()
    {
        if (npcUpgradePanel != null)
        {
            npcUpgradePanel.SetActive(true);
        }
    }

    public void CloseNPCUpgradePanel()
    {
        if (npcUpgradePanel != null)
        {
            npcUpgradePanel.SetActive(false);
        }
    }
}
