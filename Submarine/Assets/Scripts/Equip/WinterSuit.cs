using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Equipment/WinterSuit")]
public class WinterSuit : EquipmentItem
{
    [Range(0f, 1f)]
    public float resistanceAmount = 0.2f; // 예: 0.2면 20% 저항

    public override void ApplyEffect(GameObject player)
    {
        var tempGimmick = GameObject.FindObjectOfType<TemperatureGimmick>();
        if (tempGimmick != null)
        {
            tempGimmick.ApplyColdResistance(resistanceAmount);
            Logger.Log("[방한복] 체온 감소 속도 저항 적용됨: " + resistanceAmount);
        }
        else
        {
            Logger.Log("[방한복] TemperatureGimmick 없음 (스테이지 영향 X)");
        }
    }
}
