using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Ore : InteractableBase
{
    public Item oreItem;
    public int yieldAmount = 1;
    public int oreTier = 1; // 필요한 곡괭이 등급

    private OreObject oreObject; // OreSpawner 관리용

    private void Start()
    {
        InteractionManager.instance.Register(this);
        oreObject = GetComponent<OreObject>();
    }

    private void OnDestroy()
    {
        InteractionManager.instance.Unregister(this);
    }

    public override string GetHintText()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
            return "";

        Pickaxe pickaxe = player.GetComponentInChildren<Pickaxe>();
        if (pickaxe == null || !pickaxe.enabled)
        {
            return "곡괭이를 착용해야 채굴할 수 있습니다.";
        }
        else if (!CanMineWith(pickaxe.pickaxeTier))
        {
            return "곡괭이 등급이 부족합니다.";
        }

        return $"[Space] {oreItem.itemName} 채굴";
    }

    public override void Interact()
    {
        // 아무 것도 하지 않음. 실제 채광은 Pickaxe.cs에서 수행
        // 힌트는 계속 제공
    }

    public bool CanMineWith(int pickaxeTier)
    {
        return pickaxeTier >= oreTier;
    }

    public void Mine()
    {
        bool success = InventoryManager.Instance.AddItem(oreItem, yieldAmount);
        if (success)
        {
            NotificationManager.Instance.ShowPickup(oreItem, yieldAmount);

            if (oreObject != null)
            {
                oreObject.OnMined(); // OreSpawner에 재생성 요청
            }

            Destroy(gameObject);
        }
    }

    public override int Priority => 1; // 아이템보다 우선순위 낮게 설정 가능
}
