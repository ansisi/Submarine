using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUIController : MonoBehaviour
{
    public static EquipmentUIController Instance { get; private set; }

    public List<Image> equipmentSlots;
    public float selectedScale = 1.2f;
    public float unselectedScale = 0.9f;
    public float unselectedZOffset = 10f; // 선택 안 된 장비는 Z축 +10

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
        UpdateUI(0); // 시작은 첫 번째 장비 선택
    }

    public void UpdateUI(int selectedIndex)
    {
        for (int i = 0; i < equipmentSlots.Count; i++)
        {
            if (i == selectedIndex)
            {
                equipmentSlots[i].transform.localScale = Vector3.one * selectedScale;
                equipmentSlots[i].transform.localPosition = new Vector3(
                    equipmentSlots[i].transform.localPosition.x,
                    equipmentSlots[i].transform.localPosition.y,
                    0f
                );
                equipmentSlots[i].color = Color.white;
            }
            else
            {
                equipmentSlots[i].transform.localScale = Vector3.one * unselectedScale;
                equipmentSlots[i].transform.localPosition = new Vector3(
                    equipmentSlots[i].transform.localPosition.x,
                    equipmentSlots[i].transform.localPosition.y,
                    unselectedZOffset
                );
                equipmentSlots[i].color = Color.gray;
            }
        }
    }
}
