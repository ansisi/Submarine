using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrade/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    public string upgradeID;
    public string upgradeName;
    [TextArea]
    public string description;
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
