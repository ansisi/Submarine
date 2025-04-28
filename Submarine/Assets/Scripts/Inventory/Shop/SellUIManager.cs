using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellUIManager : MonoBehaviour
{
    public static SellUIManager Instance;

    public GameObject slotPrefab;
    public Transform slotParent;

    private SellSlotUI[] slotUIs;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        var slots = SellManager.Instance.GetAllSlots();
        slotUIs = new SellSlotUI[slots.Count];

        for (int i = 0; i < slots.Count; i++)
        {
            GameObject obj = Instantiate(slotPrefab, slotParent);
            slotUIs[i] = obj.GetComponent<SellSlotUI>();
            slotUIs[i].Setup(i); // ½½·Ô ÀÎµ¦½º Àü´Þ
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        var slots = SellManager.Instance.GetAllSlots();

        for (int i = 0; i < slots.Count; i++)
        {
            slotUIs[i].UpdateSlotUI(slots[i]);
        }
    }
}
