using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatItemAdder : MonoBehaviour
{
    [Header("디버그용 아이템 프리셋")]
    public Item turretItem;         // 일반 포탑
    public Item laserTurretItem;    // 레이저 포탑
    public Item bombItem;           // 폭탄
    public Item shieldTurretItem;   // 쉴드 포탑

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            AddDebugItems();
        }
    }

    private void AddDebugItems()
    {
        if (InventoryManager.Instance == null)
        {
            Logger.LogWarning("InventoryManager 인스턴스가 존재하지 않습니다.");
            return;
        }

        TryAddItem(turretItem, 3);
        TryAddItem(laserTurretItem, 3);
        TryAddItem(bombItem, 3);
        TryAddItem(shieldTurretItem, 3);

        Logger.Log("치트 아이템이 인벤토리에 추가되었습니다.");
    }

    private void TryAddItem(Item item, int quantity)
    {
        if (item == null)
        {
            Logger.LogWarning("아이템이 비어 있습니다.");
            return;
        }

        InventoryManager.Instance.AddItem(item, quantity);
    }
}
