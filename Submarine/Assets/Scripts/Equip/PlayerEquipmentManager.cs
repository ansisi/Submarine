using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Harpoon,
    Hook,
    Pickaxe
}

[System.Serializable]
public class EquipmentSlot
{
    public EquipmentType type;
    public GameObject prefab;
}

public class PlayerEquipmentManager : MonoBehaviour
{
    public static PlayerEquipmentManager Instance { get; private set; }

    public List<EquipmentSlot> equipmentSlots; // 에디터에서 지정
    public Transform handTransform; // 장비를 들 손 위치

    private GameObject currentEquipped;
    private int currentIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Equip(currentIndex);
    }

    private void Update()
    {
        // 숫자키로 전환
        if (Input.GetKeyDown(KeyCode.Alpha1)) Equip(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) Equip(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) Equip(2);

        // 마우스 휠
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) Equip((currentIndex + 1) % equipmentSlots.Count);
        else if (scroll < 0f) Equip((currentIndex - 1 + equipmentSlots.Count) % equipmentSlots.Count);
    }

    void Equip(int index)
    {
        if (index < 0 || index >= equipmentSlots.Count) return;
        currentIndex = index;

        // 기존 장비 제거
        if (currentEquipped != null)
            Destroy(currentEquipped);

        // 새 장비 생성 및 장착
        EquipmentSlot slot = equipmentSlots[currentIndex];
        currentEquipped = Instantiate(slot.prefab, handTransform);
    }

    public EquipmentType GetCurrentEquipmentType()
    {
        return equipmentSlots[currentIndex].type;
    }
}
