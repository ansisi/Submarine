using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquipmentItem : ScriptableObject
{
    public string itemName;
    public Sprite icon;

    // 각 장비마다 다른 효과를 구현하도록 추상 메서드로 정의
    public abstract void ApplyEffect(GameObject player);
}
