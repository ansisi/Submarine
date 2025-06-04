using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public QuestSO data;
    public int currentIndex = 0;

    public Quest(QuestSO so)
    {
        data = so;
    }

    public QuestStepData CurrentStep =>
        (currentIndex < data.steps.Count) ? data.steps[currentIndex] : null;

    public bool CompleteCurrentStep()
    {
        if (CurrentStep == null) return false;
        currentIndex++;
        return true;
    }

    public bool IsFinished => currentIndex >= data.steps.Count;
}
