using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ore : InteractableBase
{
    public Item oreItem;
    public int yieldAmount = 1;

    private void Start()
    {
        InteractionManager.instance.Register(this);
    }

    private void OnDestroy()
    {
        InteractionManager.instance.Unregister(this);
    }

    public override string GetHintText()
    {
        return $"[Space] {oreItem.itemName} 채굴";
    }

    public override void Interact()
    {
        bool success = InventoryManager.instance.AddItem(oreItem, yieldAmount);
        if (success)
            Destroy(gameObject);
    }

    public override int Priority => 1; // 아이템보다 우선순위 낮게 설정 가능
}
