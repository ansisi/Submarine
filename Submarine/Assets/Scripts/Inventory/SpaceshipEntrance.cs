using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipEntrance : InteractableBase
{
    public GameObject spaceshipUI; // 우주선 내부를 보여주는 UI (캔버스 안 이미지 등)

    public GameObject craftingUIPanel;
    public GameObject craftingDetailUIPanel;


    private void Start()
    {
        InteractionManager.instance.Register(this);
        spaceshipUI.SetActive(false); // 처음엔 안 보이게

        if (craftingUIPanel != null) craftingUIPanel.SetActive(false);
        if (craftingDetailUIPanel != null) craftingDetailUIPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        InteractionManager.instance.Unregister(this);
    }

    public override string GetHintText()
    {
        return "[Space] 우주선에 들어가기";
    }

    public override void Interact()
    {
        spaceshipUI.SetActive(true); // UI 창 켜기
        // 필요하면 플레이어 조작 막기, 시점 고정 등 추가 가능
        InteractionManager.instance.SetInteractionLocked(true);

        if (craftingUIPanel != null) craftingUIPanel.SetActive(false);
        if (craftingDetailUIPanel != null) craftingDetailUIPanel.SetActive(false);
    }

    private void Update()
    {
        if (spaceshipUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            spaceshipUI.SetActive(false);
            // 플레이어 조작 다시 활성화 등

            if (craftingUIPanel != null) craftingUIPanel.SetActive(false);
            if (craftingDetailUIPanel != null) craftingDetailUIPanel.SetActive(false);

            InteractionManager.instance.SetInteractionLocked(false);
        }
    }

    public override int Priority => 10; // 우선순위 낮게 하면 아이템보다 뒤로 밀릴 수 있음
}
