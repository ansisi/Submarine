using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [Header("순차 실행할 퀘스트 SO들")]
    public List<QuestSO> questSOList;

    private int currentQuestIndex = 0;
    private Quest activeQuest;

    private void Start()
    {
        if (questSOList == null || questSOList.Count == 0)
        {
            return;
        }

        // 첫 번째 퀘스트부터 시작
        StartQuest(currentQuestIndex);
    }

    private void StartQuest(int index)
    {
        // UI가 숨겨져 있을 수 있으니 켜 주기
        QuestUI.Instance.gameObject.SetActive(true);

        // 새로운 Quest 객체 생성하고 UI 세팅
        activeQuest = new Quest(questSOList[index]);
        QuestUI.Instance.Setup(activeQuest);

        // 이벤트 구독
        QuestEventSystem.OnQuestAction += HandleQuestAction;

    }

    private void HandleQuestAction(QuestActionType type, string param)
    {
        var step = activeQuest.CurrentStep;
        if (step == null) return;

        bool typeMatches = step.actionType == type;
        bool paramMatches = string.IsNullOrEmpty(step.parameter)
                            // 파라미터를 비워 두면 어떤 값이나 허용
                            || step.parameter == param;

        if (typeMatches && paramMatches)
        {
            activeQuest.CompleteCurrentStep();
            QuestUI.Instance.UpdateOnStepComplete(activeQuest.currentIndex);

            if (activeQuest.IsFinished)
                FinishCurrentQuest();
        }
    }

    private void FinishCurrentQuest()
    {
        // 이번 퀘스트 이벤트 언구독
        QuestEventSystem.OnQuestAction -= HandleQuestAction;

        currentQuestIndex++;

        // 다음 퀘스트가 있으면 시작, 없으면 UI 닫기
        if (currentQuestIndex < questSOList.Count)
        {
            StartQuest(currentQuestIndex);
        }
        else
        {
            QuestUI.Instance.Hide();
        }
    }
}
