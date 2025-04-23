using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeTabButton : MonoBehaviour
{
    public UpgradeData upgradeData; // 인스펙터에 연결할 ScriptableObject

    public void OnClick()
    {
        UpgradeUIManager ui = FindObjectOfType<UpgradeUIManager>();
        ui.SetUpgradeData(upgradeData);
    }
}
