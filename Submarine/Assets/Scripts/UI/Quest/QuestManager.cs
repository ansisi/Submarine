using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [Header("순차 실행할 퀘스트 SO들")]
    public List<QuestSO> questSOList;

    [Header("Dialogue UI References")]
    public GameObject dialoguePanel;      // QuestDialoguePanel 오브젝트
    public TextMeshProUGUI dialogueText;

    private int currentQuestIndex = 0;
    private Quest activeQuest;
    private bool waitingForDialogueConfirm = false;

    [Header("Navigation")]
    public OffScreenIndicator navigationIndicator;
    public Transform spaceshipTransform;  // 씬에 있는 우주선 Transform
    public Transform npcTransform;        // 씬에 있는 NPC Transform

    // ▶ 추가: 어떤 스텝의 긴 설명을 이미 한 번 보여줬는지 기록
    private HashSet<int> shownLongSteps = new HashSet<int>();

    private void Start()
    {
        if (questSOList == null || questSOList.Count == 0)
            return;

        // 대사 패널은 기본 숨김
        dialoguePanel.SetActive(false);

        if (navigationIndicator != null && spaceshipTransform != null)
        {
            navigationIndicator.target = spaceshipTransform;
            navigationIndicator.gameObject.SetActive(true);
        }

        // 첫 번째 퀘스트부터 시작
        StartQuest(currentQuestIndex);
    }

    private void Update()
    {
        if (waitingForDialogueConfirm)
        {
            // 스페이스 누르면 대사창을 닫고, 그 스텝을 해금(Overlay 해제) 처리
            if (Input.GetKeyDown(KeyCode.Space))
            {
                dialoguePanel.SetActive(false);
                waitingForDialogueConfirm = false;

                Time.timeScale = 1f;

                // 이제 대사만 닫았고, Overlay 해제는 이미 ApplyPrecompletedSteps에서 했으므로
                // 행동을 다시 해야만 스텝이 진짜 완료됩니다.
            }
        }
    }

    private void StartQuest(int index)
    {
        // 퀘스트 UI (체크박스 목록)가 꺼져 있을 수 있으니 다시 켜 주기
        QuestUI.Instance.gameObject.SetActive(true);

        // 새로운 Quest 객체 생성하고 UI 세팅
        activeQuest = new Quest(questSOList[index]);
        QuestUI.Instance.Setup(activeQuest);

        // 이벤트 구독
        QuestEventSystem.OnQuestAction += HandleQuestAction;

        // 할당 전에 이미 완료된 스텝이 있으면 처리
        ApplyPrecompletedSteps();
    }

    private void ApplyPrecompletedSteps()
    {
        bool again;
        do
        {
            again = false;
            var step = activeQuest.CurrentStep;
            if (step == null) break;  // 퀘스트가 완전히 끝난 경우

            int idx = activeQuest.currentIndex;
            var uiItem = QuestUI.Instance.items[idx];
            var overlay = uiItem.transform.Find("LockedOverlay").gameObject;

            if (step.actionType == QuestActionType.NPCRescue && navigationIndicator != null && npcTransform != null)
            {
                navigationIndicator.target = npcTransform;
            }

            // 1) NPCRescue처럼 이미 완료된 행동이 있으면 자동 완료
            if (step.actionType == QuestActionType.NPCRescue && GameManager.Instance.npcRescued)
            {
                activeQuest.CompleteCurrentStep();
                QuestUI.Instance.UpdateOnStepComplete(activeQuest.currentIndex);
                again = true;
                continue;
            }

            // 2) 긴 설명이 있고, 아직 한번도 보여주지 않았으면 → 대사창 띄우고, Overlay 해제(Unlock)만
            bool hasLong = !string.IsNullOrEmpty(step.longDescription);
            bool notShown = !shownLongSteps.Contains(idx);
            if (hasLong && notShown)
            {
                // 긴 설명 띄우기
                ShowLongDescription(step.longDescription);

                // LockedOverlay 해제(Unlock)
                overlay.SetActive(false);

                // 한 번 보여줬다고 기록
                shownLongSteps.Add(idx);

                if (step.actionType == QuestActionType.NPCRescue && navigationIndicator != null && npcTransform != null)
                {
                    navigationIndicator.target = npcTransform;
                    navigationIndicator.gameObject.SetActive(true);
                }

                // 이제 대사 확인을 대기해야 하므로 루프 중단
                waitingForDialogueConfirm = true;
                break;
            }

            // 3) 그 외(긴 설명 없거나 이미 해금된 상태) → 루프 종료
        }
        while (again);
    }

    private void HandleQuestAction(QuestActionType type, string param)
    {
        // 대사 확인 중이라면 어떤 액션도 무시
        if (waitingForDialogueConfirm)
            return;

        var step = activeQuest.CurrentStep;
        if (step == null) return;

        bool typeMatches = (step.actionType == type);
        bool paramMatches = string.IsNullOrEmpty(step.parameter) || step.parameter == param;

        if (typeMatches && paramMatches)
        {
            int idx = activeQuest.currentIndex;
            var uiItem = QuestUI.Instance.items[idx];
            var overlay = uiItem.transform.Find("LockedOverlay").gameObject;

            // 1) 긴 설명이 있는 스텝이었지만 이미 해금(Unlock) 상태라면 → 스텝 완료
            //    (긴 설명은 ApplyPrecompletedSteps에서 이미 처리했으므로 여기서 다시 검사할 필요 없음)
            if (!overlay.activeSelf)
            {
                if(step.actionType == QuestActionType.NPCRescue)
                {
                    if (navigationIndicator != null && spaceshipTransform != null)
                    {
                        navigationIndicator.target = spaceshipTransform;
                    }
                }

                // 이 스텝을 완료 처리
                activeQuest.CompleteCurrentStep();
                QuestUI.Instance.UpdateOnStepComplete(activeQuest.currentIndex);

                if (activeQuest.IsFinished)
                {
                    FinishCurrentQuest();
                }
                else
                {
                    ApplyPrecompletedSteps();
                }
            }
            else
            {
                // 2) 긴 설명은 없거나, 해금되지 않은 상태라면 (정상적인 스텝 진행)
                if (string.IsNullOrEmpty(step.longDescription))
                {
                    // 그냥 바로 스텝 완료
                    activeQuest.CompleteCurrentStep();
                    QuestUI.Instance.UpdateOnStepComplete(activeQuest.currentIndex);

                    if (activeQuest.IsFinished)
                        FinishCurrentQuest();
                    else
                        ApplyPrecompletedSteps();
                }
                // (긴 설명+Overlay 활성 상태인 경우는 ApplyPrecompletedSteps가 먼저 실행되어야 하므로, 여기 오지 않음)
            }
        }
    }

    private void ShowLongDescription(string text)
    {
        dialogueText.text = text;
        dialoguePanel.SetActive(true);

        Time.timeScale = 0f;
    }

    private void FinishCurrentQuest()
    {
        // 이벤트 언구독
        QuestEventSystem.OnQuestAction -= HandleQuestAction;

        Time.timeScale = 1f;

        currentQuestIndex++;

        // 다음 퀘스트가 있으면 이어서 시작, 없으면 UI 닫기
        if (currentQuestIndex < questSOList.Count)
        {
            StartQuest(currentQuestIndex);
        }
        else
        {
            QuestUI.Instance.Hide();

            if (WaveManager.Instance.GetCurrentWave() == 0)
            {
                WaveManager.Instance.TriggerWaveStart();
            }
        }
    }

    private void OnDestroy()
    {
        QuestEventSystem.OnQuestAction -= HandleQuestAction;
    }
}
