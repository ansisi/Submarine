using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentController : MonoBehaviour
{
    public GameObject pickaxeModel; // °î±ªÀÌ ÇÁ¸®ÆÕ (¼Õ¿¡ ºÙÀÏ ¸ðµ¨)
    private HarpoonController harpoonController;
    private HookController hookController;
    private Pickaxe pickaxeScript;

    private int currentIndex = 0;

    private void Awake()
    {
        harpoonController = GetComponent<HarpoonController>();
        hookController = GetComponent<HookController>();
        pickaxeScript = GetComponent<Pickaxe>();
    }

    private void Start()
    {
        SwitchEquipment(0); // ±âº» Àåºñ ¼³Á¤
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchEquipment(0); // Harpoon
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchEquipment(1); // Hook
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchEquipment(2); // Pickaxe

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) SwitchEquipment((currentIndex + 1) % 3);
        else if (scroll < 0f) SwitchEquipment((currentIndex - 1 + 3) % 3);

        EquipmentUIController.Instance.UpdateUI(currentIndex);
    }

    private void SwitchEquipment(int index)
    {
        currentIndex = index;

        harpoonController.enabled = (index == 0);
        hookController.enabled = (index == 1);
        pickaxeScript.enabled = (index == 2);

        pickaxeModel.SetActive(index == 2); // °î±ªÀÌ ¸ðµ¨Àº PickaxeÀÏ ¶§¸¸ º¸ÀÌ±â
    }

    public bool IsPickaxeEquipped()
    {
        return currentIndex == 2;
    }
}
