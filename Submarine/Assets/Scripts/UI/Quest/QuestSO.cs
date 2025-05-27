using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Tutorial Quest")]
public class QuestSO : ScriptableObject
{
    public List<QuestStepData> steps;
}
