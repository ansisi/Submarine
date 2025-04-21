using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : InteractableBase
{
    public Item item;
    [Min(1)]
    public int quantity = 1;

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
        return $"[Space] {item.itemName} 줍기";
    }

    public override void Interact()
    {
        bool success = InventoryManager.Instance.AddItem(item, quantity);
        if (success)
            Destroy(gameObject);
    }

    public override int Priority => 0; // 우선순위 높음
}
