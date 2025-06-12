using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentController : MonoBehaviour
{
    public GameObject harpoonGunModel;
    public GameObject hookGunModel;
    public GameObject pickaxeModel; // 곡괭이 프리팹 (손에 붙일 모델)
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
        SwitchEquipment(0); // 기본 장비 설정
    }

    private void Update()
    {
        // 후크나 작살이 사용 중이면 입력 무시
        if ((hookController != null && hookController.isHookActive) || 
            (harpoonController != null && harpoonController.isHarpoonActive))
        {
            return;
        }

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
        AudioManager.Instance.PlaySFX("select02");

        currentIndex = index;

        harpoonController.enabled = (index == 0);
        hookController.enabled = (index == 1);
        pickaxeScript.enabled = (index == 2);

        harpoonGunModel.SetActive(index == 0);
        hookGunModel.SetActive(index == 1);
        pickaxeModel.SetActive(index == 2); // 곡괭이 모델은 Pickaxe일 때만 보이기

        QuestEventSystem.Raise(QuestActionType.EquipSlot, index.ToString());
    }

    public bool IsPickaxeEquipped()
    {
        return currentIndex == 2;
    }
}
