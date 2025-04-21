using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager instance;

    public List<CraftingRecipe> recipes; // 등록된 모든 조합법

    private void Awake()
    {
        instance = this;
    }

    public bool CanCraft(CraftingRecipe recipe)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            int count = InventoryManager.Instance.slots
                .Where(slot => !slot.IsEmpty && slot.item == ingredient.item)
                .Sum(slot => slot.quantity);

            if (count < ingredient.quantity)
                return false;
        }
        return true;
    }

    public void Craft(CraftingRecipe recipe)
    {
        if (!CanCraft(recipe)) return;

        // 재료 차감
        foreach (var ingredient in recipe.ingredients)
        {
            int remaining = ingredient.quantity;
            foreach (var slot in InventoryManager.Instance.slots)
            {
                if (!slot.IsEmpty && slot.item == ingredient.item)
                {
                    int used = Mathf.Min(slot.quantity, remaining);
                    slot.quantity -= used;
                    remaining -= used;
                    if (slot.quantity <= 0) slot.ClearSlot();

                    if (remaining <= 0) break;
                }
            }
        }

        InventoryManager.Instance.AddItem(recipe.resultItem, recipe.resultQuantity);
        InventoryUIManger.instance.UpdateUI();
    }
}
