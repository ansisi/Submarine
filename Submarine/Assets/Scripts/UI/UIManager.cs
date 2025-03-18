using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public TextMeshProUGUI steelText;
    public TextMeshProUGUI screwNailText;
    public TextMeshProUGUI semiconductorText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void UpdateResourceUI(PartType partType, int currentAmount, int requiredAmount)
    {
        string text = $"{currentAmount} / {requiredAmount}";

        switch (partType)
        {
            case PartType.Steel:
                steelText.text = $"{currentAmount} / {requiredAmount}";
                break;
            case PartType.ScrewNail:
                screwNailText.text = $"{currentAmount} / {requiredAmount}";
                break;
            case PartType.Semiconductor:
                semiconductorText.text = $"{currentAmount} / {requiredAmount}";
                break;
        }
    }
}
