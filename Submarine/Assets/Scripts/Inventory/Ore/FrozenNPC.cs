using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenNPC : InteractableBase
{
    public int requiredPickaxeTier = 1;
    private int rescueAttempts = 0;

    private void Start()
    {
        InteractionManager.instance.Register(this);
    }

    private void OnDestroy()
    {
        InteractionManager.instance.Unregister(this);
    }

    public override string GetHintText()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return "";

        Pickaxe pickaxe = player.GetComponentInChildren<Pickaxe>();
        if (pickaxe == null || !pickaxe.enabled)
            return "곡괭이를 착용해야 구조할 수 있습니다.";
        else if (pickaxe.pickaxeTier < requiredPickaxeTier)
            return "곡괭이 등급이 부족합니다.";

        if (rescueAttempts >= 2)
            return "[Space] NPC 구조하기";

        return $"구출 시도 하기";
    }

    public override void Interact()
    {
        if (rescueAttempts >= 3)
        {
            GameManager.Instance.MarkNPCRescued();
            GameUIController.Instance.UnlockNPCUpgradeUI();
            Destroy(gameObject);

            QuestEventSystem.Raise(QuestActionType.NPCResuce);
        }
        else
        {
            rescueAttempts++;
        }
    }

    public override int Priority => 2;
}
