using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingDetailUI : MonoBehaviour
{
    public Image result;
    public Image resultIcon;
    public TextMeshProUGUI resultName;
    public Button craftButton;

    public Transform ingredientParent;
    public GameObject ingredientSlotPrefab;

    private CraftingRecipe currentRecipe;

    private void Start()
    {
        result.gameObject.SetActive(false);
        resultName.gameObject.SetActive(false);
    }

    public void ShowRecipe(CraftingRecipe recipe)
    {
        currentRecipe = recipe;

        resultName.gameObject.SetActive(true);
        result.gameObject.SetActive(true);

        resultIcon.sprite = recipe.resultItem.icon;
        resultName.text = recipe.resultItem.itemName;


        foreach (Transform child in ingredientParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var ing in recipe.ingredients)
        {
            var obj = Instantiate(ingredientSlotPrefab, ingredientParent);
            var icon = obj.transform.Find("Icon").GetComponent<Image>();
            var qty = obj.transform.Find("Quantity").GetComponent<TextMeshProUGUI>();

            icon.sprite = ing.item.icon;
            qty.text = ing.quantity.ToString();
        }

        craftButton.interactable = CraftingManager.instance.CanCraft(recipe);
    }

    public void OnCraftButtonPressed()
    {
        if (currentRecipe != null)
        {
            CraftingManager.instance.Craft(currentRecipe);
            ShowRecipe(currentRecipe); // UI °»½Å
        }
    }
}
