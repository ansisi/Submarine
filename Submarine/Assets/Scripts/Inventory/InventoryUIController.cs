using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    public static InventoryUIController instance;

    public GameObject inventoryPanel;  // 인벤토리 전체 UI 패널
    private bool isOpen = false;

    private void Awake()
    {
        instance = this;
        inventoryPanel.SetActive(false); // 시작 시 꺼져있게
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            // 퀘스트용 이벤트 발송
            QuestEventSystem.Raise(QuestActionType.OpenInventory);
        }

    }

}
