using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpaceshipEntrance : InteractableBase
{
    public GameObject spaceshipUI; // 우주선 내부를 보여주는 UI (캔버스 안 이미지 등)

    public GameObject craftingUIPanel;
    public GameObject craftingDetailUIPanel;

    public Button closeButton;

    public Item item;
    private bool IsEntrance = false;


    private void Start()
    {
        InteractionManager.instance.Register(this);
        spaceshipUI.SetActive(false); // 처음엔 안 보이게

        if (craftingUIPanel != null) craftingUIPanel.SetActive(false);
        if (craftingDetailUIPanel != null) craftingDetailUIPanel.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseSpaceshipUI);
    }

    private void OnDestroy()
    {
        InteractionManager.instance.Unregister(this);
    }

    public override string GetHintText()
    {
        if (!GameManager.Instance.npcRescued)
            return "NPC를 구출하지 않아 우주선에 들어갈 수 없습니다.";

        return "[Space] 우주선에 들어가기";
    }

    public override void Interact()
    {
        if (!GameManager.Instance.npcRescued)
        {
            Logger.Log("NPC가 얼어 있어 우주선에 진입할 수 없습니다.");
            // 여기에 UI 메시지를 띄우거나 효과음 등을 추가해도 좋음
            return;
        }

        spaceshipUI.SetActive(true); // UI 창 켜기
        // 필요하면 플레이어 조작 막기, 시점 고정 등 추가 가능
        InteractionManager.instance.SetInteractionLocked(true);

        if (craftingUIPanel != null) craftingUIPanel.SetActive(false);
        if (craftingDetailUIPanel != null) craftingDetailUIPanel.SetActive(false);

        QuestEventSystem.Raise(QuestActionType.EnterShip);

        if(!IsEntrance)
        {
            QuestEventSystem.Raise(QuestActionType.EnterShip);
            InventoryManager.Instance.AddItem(item, 1);
            IsEntrance = true;
        }
    }

    private void CloseSpaceshipUI()
    {
        spaceshipUI.SetActive(false);

        if (craftingUIPanel != null) craftingUIPanel.SetActive(false);
        if (craftingDetailUIPanel != null) craftingDetailUIPanel.SetActive(false);
        InteractionManager.instance.SetInteractionLocked(false);
    }

    public override int Priority => 10; // 우선순위 낮게 하면 아이템보다 뒤로 밀릴 수 있음
}
