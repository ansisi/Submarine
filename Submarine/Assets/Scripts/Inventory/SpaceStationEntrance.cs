using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpaceStationEntrance : InteractableBase
{
    public GameObject spaceStationUI; // 우주정거장 내부 UI (예: 캔버스 안 창들)

    public GameObject ShopUIPanel;

    public Button closeButton;

    private void Start()
    {
        InteractionManager.instance.Register(this);
        spaceStationUI.SetActive(false); // 처음에는 비활성화

        if (ShopUIPanel != null) ShopUIPanel.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseSpaceStationUI);
    }

    private void OnDestroy()
    {
        InteractionManager.instance.Unregister(this);
    }

    public override string GetHintText()
    {
        return "[Space] 우주정거장에 들어가기";
    }

    public override void Interact()
    {
        spaceStationUI.SetActive(true); // 우주정거장 UI 열기
        InteractionManager.instance.SetInteractionLocked(true);

        if (ShopUIPanel != null) ShopUIPanel.SetActive(false);
    }

    private void CloseSpaceStationUI()
    {
        spaceStationUI.SetActive(false);
        InteractionManager.instance.SetInteractionLocked(false);

        if (ShopUIPanel != null) ShopUIPanel.SetActive(false);
        
        ShopManager.Instance.CloseShop();
    }

    public override int Priority => 10;
}
