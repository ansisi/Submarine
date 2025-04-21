using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeLevelUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI levelText;  // 단계 텍스트
    public TextMeshProUGUI goldCostText;  // 골드 비용 텍스트
    public Transform materialContainer;  // 재료 아이템 표시용
    public GameObject materialPrefab;  // 재료 UI 프리팹

    private UpgradeLevelData levelData;

    // 업그레이드 단계 데이터 설정
    public void SetUpgradeLevelData(UpgradeLevelData data)
    {
        levelData = data;
        levelText.text = "Level " + data.level;
        goldCostText.text = "Gold: " + data.goldCost.ToString();

        // 재료 항목 UI 생성
        foreach (var material in data.materialRequirements)
        {
            CreateMaterialItem(material);
        }
    }

    // 재료 아이템 UI 생성
    private void CreateMaterialItem(UpgradeMaterialRequirement material)
    {
        GameObject materialItem = Instantiate(materialPrefab, materialContainer);
        MaterialItemUI materialUI = materialItem.GetComponent<MaterialItemUI>();
        materialUI.SetMaterialData(material);
    }
}