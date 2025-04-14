using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUIManager : MonoBehaviour
{
    public Transform recipeButtonParent;   // 버튼들이 들어갈 부모
    public GameObject recipeButtonPrefab;  // 프리팹 (이름, 아이콘, 버튼 포함)

    private void Start()
    {
        foreach (var recipe in CraftingManager.instance.recipes)
        {
            GameObject obj = Instantiate(recipeButtonPrefab, recipeButtonParent);

            // 버튼 이름 설정
            TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.text = recipe.resultItem.itemName;

            // 버튼 이벤트 연결
            Button button = obj.GetComponentInChildren<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() =>
                {
                    CraftingManager.instance.TryCraft(recipe);
                });
            }
        }
    }
}
