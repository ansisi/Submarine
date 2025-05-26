using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestStepData
{
    public QuestActionType actionType;  // 수행할 액션 타입
    public string parameter;            // 예: 슬롯 번호, 자원 이름 등
    public string description;          // UI에 표시할 텍스트
}

[CreateAssetMenu(menuName = "Quest/Tutorial Quest")]
public class QuestSO : ScriptableObject
{
    public List<QuestStepData> steps;
}
