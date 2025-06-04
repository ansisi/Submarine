using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestActionType
{
    Move,
    EquipSlot,        // parameter: slot index ("1", "2", ...)
    CollectResource, // parameter: resource name
    OpenInventory,
    OpenPauseMenu,
    NPCRescue,
    PurchaseItem,   // 상점 이용
    CraftItem,      // 제작
    MineOre,        // 채굴
    EnterShip,      // 우주선 입장
    UseConsumable,  // 소비 아이템 사용
    InstallTurret   // 포탑 설치
    // ... 필요에 따라 추가
}

public static class QuestEventSystem
{
    public static event Action<QuestActionType, string> OnQuestAction;

    public static void Raise(QuestActionType type, string param = "")
    {
        OnQuestAction?.Invoke(type, param);
    }
}
