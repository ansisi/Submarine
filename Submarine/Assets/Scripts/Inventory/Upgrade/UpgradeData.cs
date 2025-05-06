using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum UpgradeType
{
    Radar,
    Spaceship,
    OxygenTank,
    Inventory,
    AutoRepair
    // 등 추가 예정 업그레이드 항목
}

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrade/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    public UpgradeType upgradeType;
    public string upgradeName;
    public Sprite icon;

    public List<UpgradeLevelData> levels;
}

[System.Serializable]
public class UpgradeLevelData
{
    public int level; // 1 ~ 3
    public int goldCost; // 비용
    [TextArea]
    public string levelDescription; // 효과 설명

    public List<UpgradeMaterialRequirement> materialRequirements;
}

[System.Serializable]
public class UpgradeMaterialRequirement
{
    public Item item;
    public int amount;
}
