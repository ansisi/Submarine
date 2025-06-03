using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager instance;

    public GameObject slotPrefab;   // 슬롯 UI 프리팹
    public Transform slotParent;    // 슬롯들을 자식으로 배치할 부모 오브젝트

    private InventorySlotUI[] slotUIs;   // 슬롯 UI 스크립트 배열

    

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InventoryManager inventory = InventoryManager.Instance;
        slotUIs = new InventorySlotUI[inventory.slots.Count];

        // 슬롯 프리팹을 인벤토리 크기만큼 생성
        for (int i = 0; i < inventory.slots.Count; i++)
        {
            GameObject obj = Instantiate(slotPrefab, slotParent);
            slotUIs[i] = obj.GetComponent<InventorySlotUI>();
        }

        UpdateUI(); // 시작 시 UI 초기화
    }

    // 모든 슬롯 UI 갱신
    public void UpdateUI()
    {
        InventoryManager inventory = InventoryManager.Instance;
        for (int i = 0; i < inventory.slots.Count; i++)
        {
            var slot = inventory.slots[i];
            slotUIs[i].UpdateSlotUI(slot.item, slot.quantity);
        }
    }

    // 슬롯 UI를 동적으로 확장하는 함수
    public void ExpandSlotUI(int addedCount)
    {
        // 기존 배열을 List로 바꿔서 동적으로 슬롯 UI를 추가
        List<InventorySlotUI> uiList = new List<InventorySlotUI>(slotUIs);

        for (int i = 0; i < addedCount; i++)
        {
            GameObject obj = Instantiate(slotPrefab, slotParent);
            var slotUI = obj.GetComponent<InventorySlotUI>();
            uiList.Add(slotUI);
        }

        slotUIs = uiList.ToArray();
    }
}
