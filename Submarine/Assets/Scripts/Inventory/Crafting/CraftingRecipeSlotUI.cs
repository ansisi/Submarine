using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingRecipeSlotUI : MonoBehaviour
{
    public Image icon;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Setup(CraftingRecipe recipe, UnityEngine.Events.UnityAction onClick)
    {
        icon.sprite = recipe.resultItem.icon;
        button.onClick.AddListener(onClick);
    }
}
