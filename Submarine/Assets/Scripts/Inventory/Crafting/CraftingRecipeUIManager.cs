using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingRecipeUIManager : MonoBehaviour
{
    public GameObject recipeSlotPrefab;
    public Transform recipeSlotParent;
    public CraftingDetailUI detailsUI;

    private bool isInitialized = false;

    public void Initialize()
    {
        if (isInitialized) return;

        foreach (var recipe in CraftingManager.instance.recipes)
        {
            GameObject obj = Instantiate(recipeSlotPrefab, recipeSlotParent);
            var slot = obj.GetComponent<CraftingRecipeSlotUI>();
            slot.Setup(recipe, () => detailsUI.ShowRecipe(recipe));
        }

        isInitialized = true;
    }
}
