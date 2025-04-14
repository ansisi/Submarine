using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public Item resultItem;              // 만들어질 아이템
    public int resultQuantity = 1;       // 결과 수량

    [System.Serializable]
    public class Ingredient
    {
        public Item item;
        public int quantity;
    }

    public List<Ingredient> ingredients; // 필요한 재료들
}
