using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingRecipeSlotUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI text;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Setup(CraftingRecipe recipe, UnityEngine.Events.UnityAction onClick)
    {
        icon.sprite = recipe.resultItem.icon;
        text.text = recipe.resultItem.itemName;
        button.onClick.AddListener(onClick);
    }
}
