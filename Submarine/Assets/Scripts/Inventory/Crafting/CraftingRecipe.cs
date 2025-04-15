using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public Item resultItem;           // 완성 아이템
    public int resultQuantity = 1;    // 완성 수량

    [System.Serializable]
    public class Ingredient
    {
        public Item item;
        public int quantity;
    }

    public List<Ingredient> ingredients = new List<Ingredient>(); // 재료 목록
}
