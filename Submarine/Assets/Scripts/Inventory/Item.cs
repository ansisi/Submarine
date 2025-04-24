using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;     // 아이템 이름
    public Sprite icon;         // 인벤토리 UI에 표시할 아이콘
    public bool isStackable;    // 중첩 가능한 아이템인지 여부

    public GameObject prefab; // 실제 배치할 수 있는 오브젝트 (ex. 포탑)
}
