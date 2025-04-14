using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager instance;
    public List<CraftingRecipe> recipes;

    private void Awake()
    {
        instance = this;
    }

    public bool TryCraft(CraftingRecipe recipe)
    {
        InventoryManager inventory = InventoryManager.instance;

        // 재료 있는지 확인
        foreach (var ingredient in recipe.ingredients)
        {
            int total = 0;
            foreach (var slot in inventory.slots)
            {
                if (!slot.IsEmpty && slot.item == ingredient.item)
                    total += slot.quantity;
            }

            if (total < ingredient.quantity)
            {
                Debug.Log("재료 부족: " + ingredient.item.itemName);
                return false;
            }
        }

        // 재료 제거
        foreach (var ingredient in recipe.ingredients)
        {
            int remaining = ingredient.quantity;
            foreach (var slot in inventory.slots)
            {
                if (!slot.IsEmpty && slot.item == ingredient.item)
                {
                    int take = Mathf.Min(slot.quantity, remaining);
                    slot.quantity -= take;
                    remaining -= take;

                    if (slot.quantity == 0)
                        slot.ClearSlot();

                    if (remaining <= 0)
                        break;
                }
            }
        }

        // 결과 아이템 추가
        InventoryManager.instance.AddItem(recipe.resultItem, recipe.resultQuantity);
        InventoryUIManger.instance.UpdateUI();

        Debug.Log("제작 성공: " + recipe.resultItem.itemName);
        return true;
    }
}
